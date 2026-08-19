using UnityEngine;
namespace ETEngine.TutorialSystem
{
    [CreateAssetMenu(fileName = "TutorialConfig", menuName = "ETEngine/TutorialSystem/TutorialConfig", order = 1)]
    public class TutorialSystemConfig : ScriptableObject
    {
        public Material targetMaterial;
        public Material maskMaterial;
    }
}
