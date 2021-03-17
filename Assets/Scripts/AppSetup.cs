using UnityEngine;

namespace MyProject.RuntimeInitialization {
    static class AppSetup {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Setup() {
            var s = Settings.instance;
        }
    }
}