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
     * Événement déclenché pour verrouiller/déverrouiller la caméra du joueur
     *
     * @var		mixed	onLockPlayerCamera
     */
    public event Action<bool> onLockPlayerCamera;
    public void LockPlayerCamera(bool locked)
    {
        if (onLockPlayerCamera != null)
        {
            onLockPlayerCamera(locked);
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


    /**
     * Événement déclenché quand le joueur ouvre un placard
     *
     * @var		mixed	onCupboardOpened
     */
    public event Action<GameObject> onCupboardOpened;
    public void CupboardOpened(GameObject cupboard)
    {
        if (onCupboardOpened != null)
        {
            onCupboardOpened(cupboard);
        }
    }

    /**
     * Événement déclenché quand le joueur ferme un placard
     *
     * @var		mixed	onCupboardClosed
     */
    public event Action<GameObject> onCupboardClosed;
    public void CupboardClosed(GameObject cupboard)
    {
        if (onCupboardClosed != null)
        {
            onCupboardClosed(cupboard);
        }
    }

    /**
     * Événement déclenché quand le joueur interagit avec un objet en maintenant une touche
     *
     * @var		mixed	onPlayerHoldInteracted
     */
    public event Action<string, string> onPlayerHoldInteracted;
    public void PlayerHoldInteracted(string objectName, string objectTag)
    {
        if (onPlayerHoldInteracted != null)
        {
            onPlayerHoldInteracted(objectName, objectTag);
        }
    }

    /**
     * Événement déclenché quand le joueur interagit avec n'importe quel objet (HoldInteractAnyObject)
     *
     * @var		mixed	onPlayerHoldInteractedWithAnyObject
     */
    public event Action<GameObject> onPlayerHoldInteractedWithAnyObject;
    public void PlayerHoldInteractedWithAnyObject(GameObject obj)
    {
        if (onPlayerHoldInteractedWithAnyObject != null)
        {
            onPlayerHoldInteractedWithAnyObject(obj);
        }
    }

    /**
     * Événement déclenché quand un cadenas est déverrouillé
     *
     * @var		mixed	onPadlockUnlocked
     */
    public event Action onPadlockUnlocked;
    public void PadlockUnlocked()
    {
        if (onPadlockUnlocked != null)
        {
            onPadlockUnlocked();
        }
    }

        /**
     * Événement déclenché pour déverrouiller un tiroir
     * Utiliser DrawerUnlock("drawer_id") pour déclencher le déverrouillage
     *
     * @var		mixed	onDrawerUnlocked
     */
    public event Action<string> onDrawerUnlocked;
    public void DrawerUnlock(string drawerID)
    {
        if (onDrawerUnlocked != null)
        {
            onDrawerUnlocked(drawerID);
        }
    }

    /**
     * Événement déclenché pour déverrouiller un placard
     * Utiliser CupboardUnlock("cupboard_id") pour déclencher le déverrouillage
     *
     * @var		mixed	onCupboardUnlocked
     */
    public event Action<string> onCupboardUnlocked;
    public void CupboardUnlock(string cupboardID)
    {
        if (onCupboardUnlocked != null)
        {
            onCupboardUnlocked(cupboardID);
        }
    }

    

}
