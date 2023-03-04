using Thisaislan.PersistenceEasyToDeleteInEditor.Editor.Constants;
using Thisaislan.PersistenceEasyToDeleteInEditor.Editor.ScriptableObjects.Data;
using UnityEngine;

namespace Thisaislan.PersistenceEasyToDeleteInEditor.Editor.ScriptableObjects.Settings
{
    internal class PedeSettings : ScriptableObject
    {
        
        [Header(Metadata.PedeSettingsHeaderAttr)]
        [Tooltip(Metadata.PedeSettingsTooltipAttr)]
        [Space]
        [SerializeField]
        internal PedeData pedeData;
        
    }
}
