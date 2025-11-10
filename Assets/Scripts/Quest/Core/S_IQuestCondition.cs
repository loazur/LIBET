// !Une interface pour des conditions réutilisables (ex : condition custom qui valide si un puzzle est résolu). 
// !Permet d’extraire logique complexe hors des objectives.

using UnityEngine;

/**
 * Interface pour condition personnalisée de quête.
 * Une implémentation doit pouvoir répondre à un événement et dire si la condition est remplie.
 *
 * @var		public	interfac
 * @global
 */
public interface S_IQuestCondition
{
    /**
     * Teste si la condition est satisfaite en fonction d'un event.
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, November 10th, 2025.
     * @param	questeventdata	ev	
     * @return	void
     */
    bool Evaluate(QuestEventData ev);

    /**
     * Permet d'effectuer une réinitialisation si nécessaire.
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, November 10th, 2025.
     * @return	void
     */
    void Reset();
}

