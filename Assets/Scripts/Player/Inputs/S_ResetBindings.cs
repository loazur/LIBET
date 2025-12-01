using UnityEngine;
using UnityEngine.InputSystem;

public class S_ResetBindings : MonoBehaviour
{
    //~ Gestion pour remettre à default les keybinds
    [SerializeField] private InputActionAsset _inputActions; // InputSystem
    [SerializeField] private string _targetControlScheme; // Le nom du device qu'on veut reset

    public void ResetAllBindings() //& Reset tout les bindings peut importe le scheme
    {
        foreach (InputActionMap map in _inputActions.actionMaps)
        {
            map.RemoveAllBindingOverrides();
        }
    }

    public void ResetControlSchemeBinding() //& Reset le scheme choisi
    {
        foreach(InputActionMap map in _inputActions.actionMaps)
        {
            foreach(InputAction action in map.actions)
            {
                action.RemoveBindingOverride(InputBinding.MaskByGroup(_targetControlScheme));
            }
        }
    }
}
