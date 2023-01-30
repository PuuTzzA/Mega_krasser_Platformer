using UnityEngine;

namespace DefaultNamespace
{
    
    // Exercise 3
    /// <summary>
    /// This class should act as in a game. It stores states and data of your game.
    /// Important:
    /// 1. There should be only one Instance of this class
    /// 2. Data has to be persistant between different scenes
    /// 3. Accessible from all other classes
    /// 4. Only constructed during first access (lazy initialization) 
    /// This behavior can be achieved by using the singleton pattern. Extend this class to fulfill above requirements.
    /// As example you have a rough c# singleton class.
    /// For more details you can see https://en.wikipedia.org/wiki/Singleton_pattern
    ///
    /// To store basic data that's enough. But we want to enable unity functions like instantiate or access to resources.
    /// Extend this class to enable integration in your unity project by preserving the singleton rules.
    /// You can ignore more complex issues with singletons like thread safety.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance = null;

        /// <summary>
        /// Returns active class isntance or create if not exist
        /// </summary>
        /// <returns></returns>

        private GameManager(){}
        

        public static GameManager GetInstance()
        {
            if (instance == null)
            {   
                instance = new GameManager();
            }
            return instance;
        }


        public string PlayerName;
    }
}