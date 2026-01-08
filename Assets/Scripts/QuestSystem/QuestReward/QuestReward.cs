using UnityEngine;

/**
 * Classe de base abstraite pour toutes les récompenses de quête
 * 
 * @author	Lucas
 * @since	v0.0.1
 * @version	v1.0.0	Wednesday, January 8th, 2026.
 * @abstract
 */
public abstract class QuestReward : ScriptableObject
{


    /**
     * Méthode abstraite à implémenter pour donner la récompense au joueur
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Wednesday, January 8th, 2026.
     * @access	public
     * @return	void
     */
    public abstract void GiveReward();


}
