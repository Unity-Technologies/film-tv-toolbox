using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.FilmTV.Toolbox.MultiScene.Samples
{
    /// <summary>
    /// Simple test loader for runtime config loading of multi-scene setups
    /// </summary>
    public class TestRuntimeLoader : MonoBehaviour
    {
        public MultiSceneLoader sceneConfig;

        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(this);
            Debug.Log("NOTE: Must add all scenes from package samples into project build settings to test runtime loading");

            if ( sceneConfig != null)
            {
                StartCoroutine(LoadConfig());
            }
        }

        private IEnumerator LoadConfig()
        {
            Debug.Log("Testing runtime scene config loading....");
            yield return new WaitForSeconds(2);

            Debug.Log("Loading ALL scenes from config");
            yield return new WaitForSeconds(1);
            sceneConfig.LoadAllScenes();
            yield return new WaitForSeconds(1);

            Debug.Log("Load ALL scenes - COMPLETE");
            yield return new WaitForSeconds(2);
            
            Debug.Log("Loading individual scene configs");

            yield return new WaitForSeconds(1);
            foreach( var thisConfig in sceneConfig.config)
            {
                Debug.Log("Loading scenes from config : " + thisConfig.name);
                yield return new WaitForSeconds(1);
                sceneConfig.LoadSceneConfig(thisConfig, true);
                Debug.Log("Load  scenes from config : " + thisConfig.name + " - COMPLETE");
                yield return new WaitForSeconds(2);
            }

            Debug.Log("Loading individual scene configs without unloading existing");

            yield return new WaitForSeconds(1);
            foreach (var thisConfig in sceneConfig.config)
            {
                Debug.Log("Loading scenes from config : " + thisConfig.name);
                yield return new WaitForSeconds(1);
                sceneConfig.LoadSceneConfig(thisConfig, false);
                Debug.Log("Load  scenes from config : " + thisConfig.name + " - COMPLETE");
                yield return new WaitForSeconds(2);
            }
            Debug.Log("Load individual scene config without unloading - COMPLETE");

            yield return new WaitForSeconds(1);
            Debug.Log("Test Runtime Scene loading - COMPLETE");
            yield return new WaitForSeconds(2);
        }
    }
}