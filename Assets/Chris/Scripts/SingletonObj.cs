using UnityEngine;

public class SingletonObj : MonoBehaviour {

    private static GameObject instance;

    public static GameObject Instance {
        get {
            if (instance == null) {
                instance = GameObject.Find(instance.name);
                if (instance == null) {
                    Instantiate(instance);
                }
            }
            return instance;
        }
    }

    public virtual void Awake() {
        if (instance == null) {
            instance = gameObject;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
}
