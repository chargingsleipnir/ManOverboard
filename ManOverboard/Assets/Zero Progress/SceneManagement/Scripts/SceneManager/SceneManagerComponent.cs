using UnityEngine;

namespace ZeroProgress.SceneManagementUtility
{
    public class SceneManagerComponent : MonoBehaviour
    {
        [SerializeField, Tooltip("The controller that this manager uses")]
        private SceneManagerController sceneController;

        public SceneManagerController SceneController
        {
            get { return sceneController; }
            set
            {
                if (sceneController == value)
                    return;

                sceneController = value;

                if (sceneController != null)
                    sceneController.Transitioner = Transitioner;
            }
        }
        
        [SerializeField]
        private Object transitionerObject;

        private ISceneTransitioner transitioner;

        public ISceneTransitioner Transitioner
        {
            get { return transitioner; }
            set
            {
                if (transitioner == value)
                    return;

                transitioner = value;

                if (sceneController != null)
                    sceneController.Transitioner = Transitioner;
            }
        }
        
        private void OnValidate()
        {
            if (transitionerObject == null)
                Transitioner = null;
            else
            {
                Transitioner = transitionerObject as ISceneTransitioner;

                if (Transitioner == null)
                {
                    Debug.LogError("Transition object does not implement ISceneTransitioner interface");
                    transitionerObject = null;
                }
            }
        }

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }
   
        // Use this for initialization
        void Start()
        {
            if (transitionerObject != null)
                Transitioner = transitionerObject as ISceneTransitioner;

            SceneController.TransitionToEntry();
        }
    }
}