using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Unity.VectorGraphics;
using UnityEngine;

public class SvgLoader
{
    private const bool FlipYAxis = true;
    private const VectorUtils.Alignment SpriteAlignment = VectorUtils.Alignment.Center;
    private readonly Vector2 SpritePivot = Vector2.zero;

    public class VectorSprite
    { }
    public class StaticVectorSprite : VectorSprite
    {
        public Sprite Sprite;
    }
    public class PaintableVectorSpriteGroup : VectorSprite
    {
        public string Key;
        public List<PaintableVectorSprite> Children;
    }
    public class PaintableVectorSprite
    {
        public Sprite Fill;
        public Sprite Stroke;
        public Sprite OriginalStroke;
        public Vector2 Size;
        public Vector2 Position;
    }

    private SVGParser.SceneInfo _sceneInfo;
    private VectorUtils.TessellationOptions _tesselationSettings;
    private int _pixelsPerUnit;
    private int _targetResolution;
    private ushort _gradientResolution;
    private float _strokeHalfThickness;
    private string _paintableKey;

    public SvgLoader(VectorSpriteSettings settings)
    {
        _pixelsPerUnit = settings.PixelsPerUnit;
        _targetResolution = settings.TargetResolution;
        _gradientResolution = settings.GradientResolution;
        _strokeHalfThickness = settings.StrokeHalfThickness;
        _paintableKey = settings.PaintableGroupKey;
    }

    public void ImportSVG(TextAsset textAsset)
    {
        _sceneInfo = SVGParser.ImportSVG(new StringReader(textAsset.text), ViewportOptions.OnlyApplyRootViewBox);
        _tesselationSettings = CalculateTesselationSettings(_sceneInfo.Scene.Root, _pixelsPerUnit, _targetResolution);
    }

    public async Task<List<VectorSprite>> GetSpritesArrange()
    {
        var sceneRect = _sceneInfo.SceneViewport;

        Stroke _basicStroke = new Stroke()
        {
            Color = Color.white,
            Fill = new SolidFill() { Color = Color.white, Mode = FillMode.NonZero, Opacity = 1 },
            FillTransform = new Matrix2D() { m00 = 1, m01 = 0, m02 = 0, m10 = 0, m11 = 1, m12 = 0 },
            HalfThickness = _strokeHalfThickness,
            Pattern = null,
            PatternOffset = 0,
            TippedCornerLimit = 10
        };

        var sceneMatrix = _sceneInfo.Scene.Root.Transform;
        var paintableShapes = new Dictionary<string, SceneNode>();

        foreach (var sceneNodeID in _sceneInfo.NodeIDs)
        {
            if (sceneNodeID.Key.Contains(_paintableKey))
            {
                paintableShapes.Add(sceneNodeID.Key, sceneNodeID.Value);
                //Debug.Log(sceneNodeID.Key);
            }
        }

        var vectorSprites = new List<VectorSprite>();

        //CHECK NODE OPACITY!!!!
        foreach (var rootChild in _sceneInfo.Scene.Root.Children)
        {
            var paintable = paintableShapes.FirstOrDefault(x => x.Value == rootChild);

            if (paintable.Value != null)
            {
                PaintableVectorSpriteGroup paintableGroup = new PaintableVectorSpriteGroup();
                paintableGroup.Key = paintable.Key;
                paintableGroup.Children = new List<PaintableVectorSprite>();
                var childrenShapes = GetShapes(rootChild);
                if (childrenShapes.Count == 0)
                {
                    Debug.Log("No shapes detected");
                    continue;
                }

                foreach (var childShape in childrenShapes)
                {
                    var childPaintableObject = new PaintableVectorSprite();
                    var root = new SceneNode()
                    {
                        Children = null,
                        Clipper = null,
                        Transform = sceneMatrix,
                        Shapes = new List<Shape>() { childShape }
                    };

                    //save original stroke
                    if (childShape.PathProps.Stroke != null)
                    {
                        var geometryListStroke = await Task.Run(() => VectorUtils.TessellateScene(new Scene()
                        {
                            Root = root
                        }, _tesselationSettings, _sceneInfo.NodeOpacity));

                        if (geometryListStroke.Count > 1)
                        {
                            childPaintableObject.OriginalStroke = VectorUtils.BuildSprite(
                                new List<VectorUtils.Geometry>() { geometryListStroke[1] }, sceneRect, _pixelsPerUnit,
                                SpriteAlignment, SpritePivot, _gradientResolution, FlipYAxis);
                        }
                    }

                    childShape.PathProps = new PathProperties() { Corners = PathCorner.Tipped, Head = PathEnding.Chop, Tail = PathEnding.Chop, Stroke = _basicStroke };
                    var geometryList = await Task.Run(() => VectorUtils.TessellateScene(new Scene()
                    {
                        Root = root
                    }, _tesselationSettings, _sceneInfo.NodeOpacity));

                    geometryList[0].Color = Color.white;

                    //set fill
                    childPaintableObject.Fill = VectorUtils.BuildSprite(
                        new List<VectorUtils.Geometry>() { geometryList[0] }, sceneRect, _pixelsPerUnit,
                        SpriteAlignment, SpritePivot, _gradientResolution, FlipYAxis);

                    //calculate size and position for future collider
                    var fillBounds = geometryList[0].UnclippedBounds;
                    childPaintableObject.Size = new Vector2(fillBounds.size.x / _pixelsPerUnit, fillBounds.size.y / _pixelsPerUnit);
                    var posX = (childPaintableObject.Size.x / 2) - (sceneRect.width / _pixelsPerUnit / 2) + (fillBounds.position.x / _pixelsPerUnit);
                    var posY = (sceneRect.height / _pixelsPerUnit / 2) - (childPaintableObject.Size.y / 2) - (fillBounds.position.y / _pixelsPerUnit);
                    childPaintableObject.Position = new Vector2(posX, posY);

                    //set stroke
                    if (geometryList.Count > 1)
                    {
                        geometryList[1].Color = Color.white;
                        childPaintableObject.Stroke = VectorUtils.BuildSprite(
                            new List<VectorUtils.Geometry>() { geometryList[1] }, sceneRect, _pixelsPerUnit,
                            SpriteAlignment, SpritePivot, _gradientResolution, FlipYAxis);
                    }
                    else
                    {
                        Debug.Log("NO STROKE");
                    }
                    paintableGroup.Children.Add(childPaintableObject);
                }

                vectorSprites.Add(paintableGroup);
            }
            else
            {
                StaticVectorSprite staticVectorSprite = new StaticVectorSprite();

                var geometry = await Task.Run(() => VectorUtils.TessellateScene(new Scene() { Root = rootChild },
                    _tesselationSettings, _sceneInfo.NodeOpacity));
                staticVectorSprite.Sprite = VectorUtils.BuildSprite(geometry, sceneRect, _pixelsPerUnit,
                        SpriteAlignment, SpritePivot, _gradientResolution, FlipYAxis);

                vectorSprites.Add(staticVectorSprite);
            }
        }

        return vectorSprites;
    }

    public async Task<StaticVectorSprite> GetStaticSprite()
    {
        StaticVectorSprite staticVectorSprite = new StaticVectorSprite();
        
        var geometry = await Task.Run(() => VectorUtils.TessellateScene(_sceneInfo.Scene, _tesselationSettings, _sceneInfo.NodeOpacity));
        staticVectorSprite.Sprite = VectorUtils.BuildSprite(geometry, _sceneInfo.SceneViewport, _pixelsPerUnit, SpriteAlignment, SpritePivot, _gradientResolution, FlipYAxis);

        return staticVectorSprite;
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
