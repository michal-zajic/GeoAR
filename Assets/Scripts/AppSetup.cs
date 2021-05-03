////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: AppSetup.cs
//Author : Michal Zajíc
//Year : 2021
////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

namespace MyProject.RuntimeInitialization {
    
    static class AppSetup {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        //method is called before all start methods, may be useful in future
        static void Setup() {
        }
    }
}