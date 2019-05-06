using UnityEngine;

[CreateAssetMenu(fileName = "BaseCharacter", menuName = "New Character", order = 0)]
public class BaseCharacter : ScriptableObject {

    public int Health;
    public int Speed;
    public int JumpHeight;

}
