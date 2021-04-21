using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

//stores set of level object copies
public class LevelObjectsController : MonoBehaviour
{
    private const float SortingStepZ = -.01f;
    public List<LevelObjectController> LevelObjectControllers => _levelObjectControllers;
    [SerializeField] private LevelObjectController _levelObjectPrefab;
    [SerializeField] private StaticLevelObjectController _staticLevelObjectPrefab;

    private List<LevelObjectController> _levelObjectControllers = new List<LevelObjectController>();
    private LevelObject _levelObject;
    private static SvgLoader _svgLoader = new SvgLoader();

    //instantiate and init sprites groups
    public async Task<List<PaintableSpriteGroup>> Init(LevelObject levelObject, CancellationToken token)
    {
        _levelObject = levelObject;

        _svgLoader.ImportSVG(levelObject.SvgTextAsset);

        //Debug.Log("LOADING STARTED "+ Time.time);
        //tesselate and build sprites
        List<SvgLoader.VectorSprite> vectorSprites = new List<SvgLoader.VectorSprite>();
        if (levelObject.IsStatic)
        {
            var staticSprite = await _svgLoader.GetStaticSprite();
            vectorSprites.Add(staticSprite);
        }
        else
        {
            vectorSprites = await _svgLoader.GetSpritesArrange();
        }
        //Debug.Log("LOADING ENDED " + Time.time);

        if (vectorSprites.Count == 0)
        {
            Debug.LogError("0 sprites");
            return null;
        }

        var _levelObjPosZ = SortingStepZ;
        var paintableGroups = new List<PaintableSpriteGroup>();
        for (int i = 0; i < levelObject.CopiesSettings.Count; i++)
        {
            if (token.IsCancellationRequested)
            {
                return null;
            }

            if (levelObject.IsStatic)
            {
                var staticObjInstance = Instantiate(_staticLevelObjectPrefab, transform);
                staticObjInstance.Init(vectorSprites[0] as SvgLoader.StaticVectorSprite);
                staticObjInstance.InitSettings(levelObject.CopiesSettings[i], _levelObjPosZ);
            }
            else
            {
                var objectInstance = Instantiate(_levelObjectPrefab, transform);

                objectInstance.Init(vectorSprites);
                objectInstance.InitSettings(levelObject.CopiesSettings[i], levelObject.QuestionSignSettings, _levelObjPosZ);

                paintableGroups.AddRange(objectInstance.PaintableSpriteGroups);
                _levelObjectControllers.Add(objectInstance);
            }
            _levelObjPosZ += SortingStepZ;
        }

        return paintableGroups;
    }

    public void SetColors()
    {
        foreach (var swatchesSettings in _levelObject.ColorsSettings)
        {
            var swatches = swatchesSettings.Swatches.Colors;
            Shuffle(swatches);

            int swatchIndex = 0;

            for (int i = 0; i < _levelObjectControllers.Count; i++)
            {
                //swatches count can be less than LO's count
                if (swatchIndex >= swatches.Count)
                {
                    swatchIndex = 0;
                }
                if (swatchesSettings.SearchByKey)
                {
                    _levelObjectControllers[i].SetGroupColorByKey(swatchesSettings.Key, swatches[swatchIndex]);
                }
                else
                {
                    _levelObjectControllers[i].SetGroupsColor(swatches[swatchIndex]);
                }

                swatchIndex++;
            }
        }
    }

    public void OpenObjects(bool isOpen)
    {
        _levelObjectControllers.ForEach(x => x.OpenObject(isOpen));
    }

    private void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
