﻿using System.Collections.Generic;
using System.IO;
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
        public List<PaintableVectorSprite> Children;
        public Color OriginalFillColor;
    }
    public class PaintableVectorSprite
    {
        public Sprite Fill;
        public Sprite Stroke;
        public Sprite OriginalStroke;
        public Vector2 Size;
        public Vector2 Position;
    }

    private TextAsset _textAsset;
    private SVGParser.SceneInfo _sceneInfo; 

    public SvgLoader(TextAsset textAsset)
    {
        _textAsset = textAsset;
        _sceneInfo = LoadSVGfromFile();
    }

    public List<VectorSprite> GetSpritesArrange(VectorSpriteSettings settings)
    {
        var pixelsPerUnit = settings.PixelsPerUnit;
        var targetResolution = settings.TargetResolution;
        Stroke _basicStroke = new Stroke()
        {
            Color = Color.white,
            Fill = new SolidFill() { Color = Color.white, Mode = FillMode.NonZero, Opacity = 1 },
            FillTransform = new Matrix2D() { m00 = 1, m01 = 0, m02 = 0, m10 = 0, m11 = 1, m12 = 0 },
            HalfThickness = settings.StrokeHalfThickness,
            Pattern = null,
            PatternOffset = 0,
            TippedCornerLimit = 10
        };

        var sceneMatrix = _sceneInfo.Scene.Root.Transform;
        var paintableShapes = new List<SceneNode>();

        foreach (var sceneNodeID in _sceneInfo.NodeIDs)
        {
            if (sceneNodeID.Key.Contains(settings.PaintableGroupKey))
            {
                paintableShapes.Add(sceneNodeID.Value);
            }
        }

        var vectorSprites = new List<VectorSprite>();

        foreach (var rootChild in _sceneInfo.Scene.Root.Children)
        {
            if (paintableShapes.Contains(rootChild))
            {
                PaintableVectorSpriteGroup paintableGroup = new PaintableVectorSpriteGroup();
                paintableGroup.Children = new List<PaintableVectorSprite>();
                var childrenShapes = GetShapes(rootChild);
                if (childrenShapes.Count == 0)
                {
                    Debug.Log("No shapes detected");
                    break;
                }

                paintableGroup.OriginalFillColor = (childrenShapes[0].Fill as SolidFill).Color;

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
                        var geometryListStroke = VectorUtils.TessellateScene(new Scene()
                        {
                            Root = root
                        }, CalculateTesselationSettings(root, pixelsPerUnit, targetResolution));
                        if (geometryListStroke.Count > 1)
                        {
                            childPaintableObject.OriginalStroke = VectorUtils.BuildSprite(
                                new List<VectorUtils.Geometry>() { geometryListStroke[1] }, settings.SceneRect, pixelsPerUnit,
                                SpriteAlignment, SpritePivot, settings.GradientResolution, FlipYAxis);
                        }
                    }

                    childShape.PathProps = new PathProperties() { Corners = PathCorner.Tipped, Head = PathEnding.Chop, Tail = PathEnding.Chop, Stroke = _basicStroke };
                    var geometryList = VectorUtils.TessellateScene(new Scene()
                    {
                        Root = root
                    }, CalculateTesselationSettings(root, pixelsPerUnit, targetResolution));

                    geometryList[0].Color = Color.white;

                    //set fill
                    childPaintableObject.Fill = VectorUtils.BuildSprite(
                        new List<VectorUtils.Geometry>() { geometryList[0] }, settings.SceneRect, pixelsPerUnit,
                        SpriteAlignment, SpritePivot, settings.GradientResolution, FlipYAxis);

                    //calculate size and position for future collider
                    var fillBounds = geometryList[0].UnclippedBounds;
                    childPaintableObject.Size = new Vector2(fillBounds.size.x / pixelsPerUnit, fillBounds.size.y / pixelsPerUnit);
                    var posX = (childPaintableObject.Size.x / 2) - (settings.SceneRect.width / pixelsPerUnit / 2) + (fillBounds.position.x / pixelsPerUnit);
                    var posY = (settings.SceneRect.height / pixelsPerUnit / 2) - (childPaintableObject.Size.y / 2) - (fillBounds.position.y / pixelsPerUnit);
                    childPaintableObject.Position = new Vector2(posX, posY);

                    //set stroke
                    if (geometryList.Count > 1)
                    {
                        geometryList[1].Color = Color.white;
                        childPaintableObject.Stroke = VectorUtils.BuildSprite(
                            new List<VectorUtils.Geometry>() { geometryList[1] }, settings.SceneRect, pixelsPerUnit,
                            SpriteAlignment, SpritePivot, settings.GradientResolution, FlipYAxis);
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

                var geometry = VectorUtils.TessellateScene(new Scene() { Root = rootChild },
                    CalculateTesselationSettings(rootChild, pixelsPerUnit, targetResolution));
                staticVectorSprite.Sprite = VectorUtils.BuildSprite(geometry, settings.SceneRect, pixelsPerUnit,
                        SpriteAlignment, SpritePivot, settings.GradientResolution, FlipYAxis);

                vectorSprites.Add(staticVectorSprite);
            }
        }

        return vectorSprites;
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

    private SVGParser.SceneInfo LoadSVGfromFile()
    {
        return SVGParser.ImportSVG(new StringReader(_textAsset.text));
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