using UnityEngine;

public class SandboxScript : MonoBehaviour
{
    int masterJahmi = 0;
    void Start()
    {
        masterJahmi = 14;
        Debug.Log("master Jahmi valeur à l'init : " + masterJahmi);
        Test(out masterJahmi);
        // c'est comme masterJahmi = Test(masterJahmi);
        // mais ca ne cree pas une variable en plus donc c'est plus optimisé
        Debug.Log("master Jahmi valeur à la fin : " + masterJahmi);
    }

    int Test(out int googoo) // out modifie directement la variable d'entrée
    {
        googoo = 2 + 2;
        Debug.Log("valeur googoo " +  googoo);
        return googoo;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
