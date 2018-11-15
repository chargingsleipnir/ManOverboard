using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroProgress.SceneManagementUtility
{
    public class SceneNodeCollection : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<SceneNode> sceneNodes;

        public List<SceneNode> SceneNodes
        {
            get
            {
                if (sceneNodes == null)
                    sceneNodes = new List<SceneNode>();

                return sceneNodes;
            }
            private set { sceneNodes = value; }
        }

        public Vector2 PanOffset;

        public SceneManagerController SceneManager;

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            PopulateSceneInfos();
        }

        public void PopulateSceneInfos()
        {
            if (sceneNodes == null)
                return;

            foreach (SceneNode node in sceneNodes)
            {
                if (node.SceneId == SceneManagerController.ANY_SCENE_ID)
                    node.SceneInfo = SceneManager.AnyScene;
                else
                    node.SceneInfo = SceneManager.GetSceneById(node.SceneId);
            }
        }

        public SceneNode GetNodeBySceneId(int sceneId)
        {
            return SceneNodes.Find((x) => x.SceneInfo.SceneId == sceneId);
        }
        
        public SceneNode GetNodeByScenePath(string scenePath)
        {
            return SceneNodes.Find((x) =>
                x.SceneInfo != null && x.SceneInfo.SceneAssetPath.Equals(scenePath));
        }

        public SceneNode AddSceneNode(int sceneId, Vector2 position)
        {
            SceneModel sceneInfo = null;

            if (sceneId == SceneManagementUtility.SceneManagerController.ANY_SCENE_ID)
                sceneInfo = SceneManager.AnyScene;
            else
                sceneInfo = SceneManager.GetSceneById(sceneId);

            if (sceneInfo == null)
                throw new ArgumentException("Failed to find scene under id: " + sceneId);

            SceneNode node = new SceneNode(position, sceneInfo);
            sceneNodes.Add(node);
            return node;
        }

        public void RemoveSceneNodesByPath(string path)
        {
            SceneNodes.RemoveAll((x) => 
                x.SceneInfo != null && x.SceneInfo.SceneAssetPath.Equals(path));
        }

        public void ClearNodes()
        {
            sceneNodes.Clear();
        }

        public void SetEntrySceneNode(int entrySceneId)
        {
            foreach (SceneNode node in sceneNodes)
            {
                node.IsEntryScene = node.SceneId == entrySceneId;
            }
        }

        public void UpdateActiveSceneNode()
        {
            if (sceneNodes == null)
                return;

            foreach (SceneNode node in sceneNodes)
            {
                node.IsActiveScene = (SceneManager.ActiveScene != null &&
                    node.SceneId == SceneManager.ActiveScene.SceneId);
            }
        }
    }
}
