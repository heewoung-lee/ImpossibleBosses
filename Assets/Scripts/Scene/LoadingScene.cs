using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : BaseScene
{
    UI_Loading _ui_loding;

    private static Define.Scene _nextScene;
    public override Define.Scene CurrentScene => Define.Scene.LoadingScene;

    public static void LoadNextScene(Define.Scene nextScene)
    {
        _nextScene = nextScene;
    }

    protected override void StartInit()
    {
        base.StartInit();
        StartCoroutine(LoadingSceneProcess());
    }

    public override void Clear()
    {
        throw new System.NotImplementedException();
    }

    protected override void AwakeInit()
    {
        _ui_loding = Managers.UI_Manager.ShowSceneUI<UI_Loading>();
    }




    private IEnumerator LoadingSceneProcess()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(_nextScene.ToString());
        operation.allowSceneActivation = false;
        float timer = 0f;
        while (!operation.isDone)
        {
            yield return null;
            if(operation.progress < 0.9f)
            {
                _ui_loding.LoaingSliderValue = operation.progress;
            }
            else
            {
                timer += Time.deltaTime/10f;
                _ui_loding.LoaingSliderValue = Mathf.Lerp(0.9f,1,timer);
                if (_ui_loding.LoaingSliderValue>=1.0f)
                {
                    operation.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
