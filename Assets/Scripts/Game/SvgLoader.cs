using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.VectorGraphics;
using UnityEngine;
namespace SvgLoaderModule
{
    public class VectorImage
    { }
    public class StaticVectorImage : VectorImage
    {
        public Sprite Sprite;
    }
    public class SelectableImages : VectorImage
    {
        public List<Sprite> Children;
    }

    public class SvgLoader
    {
        private const int PixelsPerUnit = 100;
        private const int TargetResolution = 500;
        private const ushort GradientResolution = 128;
        private const string SelectableKey = "selectable";
        private const bool FlipYAxis = true;
        private const VectorUtils.Alignment SpriteAlignment = VectorUtils.Alignment.Center;
        private readonly Vector2 SpritePivot = Vector2.zero;

        private SVGParser.SceneInfo _sceneInfo;
        private VectorUtils.TessellationOptions _tesselationSettings;

        public async Task ImportSVGAsync(TextAsset textAsset)
        {
            var text = textAsset.text;
            var dpi = Screen.dpi; // a hack to run SVGParser.ImportSVG() on a background thread :)
            _sceneInfo = await Task.Run(() => SVGParser.ImportSVG(new StringReader(text), ViewportOptions.OnlyApplyRootViewBox, dpi));
            _tesselationSettings = CalculateTesselationSettings(_sceneInfo.Scene.Root, PixelsPerUnit, TargetResolution);
        }

        public async Task<List<VectorImage>> GetSpritesArrange(CancellationToken token)
        {
            var sceneRect = _sceneInfo.SceneViewport;
            var sceneMatrix = _sceneInfo.Scene.Root.Transform;
            var selectableShapes = new List<SceneNode>();

            foreach (var sceneNodeID in _sceneInfo.NodeIDs)
            {
                if (sceneNodeID.Key.Contains(SelectableKey))
                {
                    selectableShapes.Add(sceneNodeID.Value);
                }
            }

            var vectorImages = new List<VectorImage>();

            foreach (var rootChild in _sceneInfo.Scene.Root.Children)
            {
                if (token.IsCancellationRequested)
                {
                    return null;
                }

                var selectable = selectableShapes.FirstOrDefault(x => x == rootChild);

                if (selectable != null)
                {
                    SelectableImages selectableImages = new SelectableImages();

                    selectableImages.Children = new List<Sprite>();

                    if (rootChild.Children.Count == 0)
                    {
                        Debug.Log("No children detected");
                        continue;
                    }

                    var sprites = await GetSpritesFromSceneNodesAsync(rootChild.Children, sceneRect);
                    selectableImages.Children.AddRange(sprites);
                    vectorImages.Add(selectableImages);
                }
                else
                {
                    StaticVectorImage staticImage = new StaticVectorImage();

                    staticImage.Sprite = await GetSpriteFromSceneNodeAsync(rootChild, sceneRect);

                    vectorImages.Add(staticImage);
                }
            }

            return vectorImages;
        }

        private async Task<Sprite> GetSpriteFromSceneNodeAsync(SceneNode node, Rect sceneRect)
        {
            var geometry = await Task.Run(() => VectorUtils.TessellateScene(
                new Scene()
                {
                    Root = node
                },
                _tesselationSettings,
                _sceneInfo.NodeOpacity));

            return VectorUtils.BuildSprite(geometry, sceneRect, PixelsPerUnit,
                    SpriteAlignment, SpritePivot, GradientResolution, FlipYAxis);
        }

        private async Task<List<Sprite>> GetSpritesFromSceneNodesAsync(IList<SceneNode> nodes, Rect sceneRect)
        {
            var geometryList = await Task.Run(() => nodes
                    .Select(node => VectorUtils.TessellateScene(
                        new Scene
                        {
                            Root = node
                        },
                        _tesselationSettings,
                        _sceneInfo.NodeOpacity))
                    .ToList()
            );

            return geometryList
                .Select(geometry => VectorUtils.BuildSprite(geometry, sceneRect, PixelsPerUnit,
                    SpriteAlignment, SpritePivot, GradientResolution, FlipYAxis))
                .ToList();
        }

        private List<Shape> GetShapes(SceneNode rootNode)
        {
            var shapes = new List<Shape>();
            if (rootNode.Children == null)
            {
                if (rootNode.Shapes != null)
                {
                    shapes.AddRange(rootNode.Shapes);
                }
            }
            else
            {
                foreach (var node in rootNode.Children)
                {
                    shapes.AddRange(GetShapes(node));
                }
            }

            return shapes;
        }

        private VectorUtils.TessellationOptions CalculateTesselationSettings(SceneNode root, float pixelsPerUnit, int targetResolution)
        {
            var bounds = VectorUtils.ApproximateSceneNodeBounds(root);
            float maxDim = Mathf.Max(bounds.width, bounds.height) / pixelsPerUnit;

            // The scene ratio gives a rough estimate of coverage % of the vector scene on the screen.
            // Higher values should result in a more dense tessellation.
            float sceneRatio = maxDim / targetResolution;

            var maxCord = Mathf.Max(0.01f, 75.0f * sceneRatio);
            var maxTangent = Mathf.Max(0.1f, 100.0f * sceneRatio);

            return new VectorUtils.TessellationOptions
            {
                StepDistance = float.MaxValue, /*Distance at which the importer generates vertices along the paths. Lower values result in a more dense tessellation.*/
                SamplingStepSize = .01f, /*The number of samples used internally to evaluate the curves. More samples = higher quality. Should be between 0 and 1 (inclusive).*/
                MaxCordDeviation = maxCord, /*The maximum distance on the cord to a straight line between to points after which more tessellation will be generated. To disable, specify float.MaxValue.*/
                MaxTanAngleDeviation = maxTangent /*The maximum angle (in degrees) between the curve tangent and the next point after which more tessellation will be generated. To disable, specify Mathf.PI/2.0f.*/
            };
        }
    }
}
