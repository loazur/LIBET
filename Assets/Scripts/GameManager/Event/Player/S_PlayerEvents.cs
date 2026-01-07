using System;
using UnityEngine;

public class S_PlayerEvents
{
    public event Action onDisablePlayerMovement;
    public void DisablePlayerMovement()
    {
        if (onDisablePlayerMovement != null) 
        {
            onDisablePlayerMovement();
        }
    }

    public event Action onEnablePlayerMovement;
    public void EnablePlayerMovement()
    {
        if (onEnablePlayerMovement != null) 
        {
            onEnablePlayerMovement();
        }
    }

    public event Action<int> onExperienceGained;
    public void ExperienceGained(int experience) 
    {
        if (onExperienceGained != null) 
        {
            onExperienceGained(experience);
        }
    }

    public event Action<int> onPlayerLevelChange;
    public void PlayerLevelChange(int level) 
    {
        if (onPlayerLevelChange != null) 
        {
            onPlayerLevelChange(level);
        }
    }

    public event Action<int> onPlayerExperienceChange;
    public void PlayerExperienceChange(int experience) 
    {
        if (onPlayerExperienceChange != null) 
        {
            onPlayerExperienceChange(experience);
        }
    }
    
    // ! ======================================================================================
    // ! ======================================================================================
    // ! ICI ON CONFIGURE LES ÉVÉNEMENTS LIÉS AUX ACTIONS DU JOUEUR POUR LE SYSTÈME DE QUÊTES !
    // ! ======================================================================================
    // ! ======================================================================================

    /**
     * Événement déclenché quand le joueur ramasse un item
     *
     * @var		mixed	onItemPickedUp
     */
    public event Action<GameObject> onItemPickedUp;
    public void ItemPickedUp(GameObject item)
    {
        if (onItemPickedUp != null)
        {
            onItemPickedUp(item);
        }
    }

    /**
     * Événement déclenché quand le joueur s'assoit sur une chaise
     *
     * @var		mixed	onPlayerSat
     */
    public event Action<GameObject> onPlayerSat;
    public void PlayerSat(GameObject chair)
    {
        if (onPlayerSat != null)
        {
            onPlayerSat(chair);
        }
    }

    /**
     * Événement déclenché quand le joueur ouvre une porte
     *
     * @var		mixed	onDoorOpened
     */
    public event Action<GameObject> onDoorOpened;
    public void DoorOpened(GameObject door)
    {
        if (onDoorOpened != null)
        {
            onDoorOpened(door);
        }
    }

    /**
     * Événement déclenché quand le joueur ferme une porte
     *
     * @var		mixed	onDoorClosed
     */
    public event Action<GameObject> onDoorClosed;
    public void DoorClosed(GameObject door)
    {
        if (onDoorClosed != null)
        {
            onDoorClosed(door);
        }
    }

    /**
     * Événement déclenché quand le menu s'ouvre (pour cacher l'UI des quêtes)
     *
     * @var		mixed	onMenuOpened
     */
    public event Action onMenuOpened;
    public void MenuOpened()
    {
        if (onMenuOpened != null)
        {
            onMenuOpened();
        }
    }

    /**
     * Événement déclenché quand le menu se ferme (pour afficher l'UI des quêtes)
     *
     * @var		mixed	onMenuClosed
     */
    public event Action onMenuClosed;
    public void MenuClosed()
    {
        if (onMenuClosed != null)
        {
            onMenuClosed();
        }
    }

    /**
     * Événement déclenché quand le joueur ramasse une clé
     *
     * @var		mixed	onKeyCollected
     */
    public event Action<GameObject, string, string> onKeyCollected;
    public void KeyCollected(GameObject key, string doorID, string keyID)
    {
        if (onKeyCollected != null)
        {
            onKeyCollected(key, doorID, keyID);
        }
    }
}
