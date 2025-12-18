# Système de Stats et d'Arbre de Talents - AstroSurvivor

## 📋 Vue d'ensemble

Ce système fournit un framework complet pour gérer les statistiques du joueur et un arbre de talents pour votre Action Roguelite / Bullet Heaven.

## 📦 Scripts inclus

### 1. **PlayerStats.cs**
Gère toutes les statistiques du joueur avec un système de modificateurs.

**Stats disponibles:**
- ❤️ **HP Max** - Points de vie maximum
- 💚 **HP** - Points de vie actuels
- 🛡️ **Shield** - Bouclier
- ⚔️ **Damage** - Dégâts de base
- 🎯 **Critical Chance** - Chance de coup critique (%)
- 💥 **Critical Damage** - Multiplicateur de dégâts critiques (%)
- ⚡ **Attack Speed** - Vitesse d'attaque (attaques/sec)
- 🔫 **Projectile Count** - Nombre de projectiles
- 📏 **Range** - Portée d'attaque

**Fonctionnalités:**
- Système de modificateurs en pourcentage
- Gestion des dégâts et du bouclier
- Calcul automatique des coups critiques
- Events pour notifier les changements de stats

### 2. **StatModifier.cs**
Définit les types de modificateurs et comment ils s'appliquent aux stats.

**Types de stats modifiables:**
- `MaxHp`, `Damage`, `CriticalChance`, `CriticalDamage`
- `AttackSpeed`, `ProjectileCount`, `Range`, `Shield`

### 3. **TalentNodeData.cs** (ScriptableObject)
Représente un nœud individuel dans l'arbre de talents.

**Configuration:**
- Nom et description du talent
- Icône
- Liste de modificateurs de stats
- Prérequis (niveau, nœuds parents)
- Points maximum investissables
- Coût en points de talent
- Position dans l'arbre (pour l'UI)

### 4. **TalentTree.cs** (ScriptableObject)
Définit un arbre de talents complet.

**Fonctionnalités:**
- Collection de tous les nœuds de talents
- Validation de la structure de l'arbre
- Navigation dans l'arbre (parents, enfants, racines)
- Configuration des points par niveau

### 5. **TalentTreeManager.cs**
Gère la progression et l'interaction avec l'arbre de talents.

**Fonctionnalités:**
- Déverrouillage de talents
- Gestion des points de talents
- Système de level up
- Sauvegarde/Chargement
- Events pour l'UI

### 6. **ExampleUsage.cs**
Script d'exemple montrant comment utiliser le système.

---

## 🚀 Guide d'utilisation rapide

### Étape 1: Configuration de base

1. **Créer un GameObject "Player"**
   - Ajouter le composant `PlayerStats`
   - Ajouter le composant `TalentTreeManager`
   - Configurer les stats de base dans l'inspecteur

2. **Créer un TalentTree ScriptableObject**
   - Clic droit dans le Project → Create → AstroSurvivor → Talent Tree
   - Nommer le fichier (ex: "MainTalentTree")
   - Configurer le nom, description, icône

3. **Créer des TalentNode ScriptableObjects**
   - Clic droit dans le Project → Create → AstroSurvivor → Talent Node
   - Pour chaque talent:
     - Configurer le nom, description, icône
     - Ajouter des modificateurs de stats
     - Définir les prérequis (niveau, parents)
     - Configurer maxPoints et pointCost

4. **Assembler l'arbre**
   - Dans le TalentTree, ajouter tous vos TalentNodes à la liste
   - Le système validera automatiquement la structure

5. **Connecter les composants**
   - Dans le TalentTreeManager, assigner:
     - Le TalentTree créé
     - La référence au PlayerStats (automatique si sur le même GameObject)

### Étape 2: Créer vos premiers talents

**Exemple 1: Talent de dégâts simple**
```
Nom: "Force brutale"
Description: "Augmente vos dégâts"
Modificateurs:
  - Type: Damage
  - Value: 10 (= +10% de dégâts)
Max Points: 5
Point Cost: 1
```

**Exemple 2: Talent multi-stats**
```
Nom: "Guerrier endurant"
Description: "Augmente vos HP et votre régénération"
Modificateurs:
  - Type: MaxHp, Value: 15
  - Type: Shield, Value: 20
Max Points: 3
Point Cost: 2
```

**Exemple 3: Talent de critique**
```
Nom: "Œil de lynx"
Description: "Améliore vos coups critiques"
Modificateurs:
  - Type: CriticalChance, Value: 5
  - Type: CriticalDamage, Value: 25
Max Points: 1
Point Cost: 3
Parent Nodes: [ID du talent précédent]
Required Level: 5
```

### Étape 3: Utilisation dans le code

```csharp
using AstroSurvivor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private TalentTreeManager talentManager;

    // Quand le joueur gagne de l'XP et passe un niveau
    void OnPlayerLevelUp()
    {
        talentManager.LevelUp(); // Donne automatiquement des points de talents
    }

    // Quand le joueur clique sur un talent dans l'UI
    void OnTalentNodeClicked(string nodeId)
    {
        bool success = talentManager.TryUnlockTalent(nodeId);
        
        if (success)
        {
            // Afficher une notification de succès
            ShowNotification("Talent débloqué!");
        }
        else
        {
            // Afficher pourquoi ça a échoué
            ShowNotification("Impossible de débloquer ce talent");
        }
    }

    // Utiliser les stats du joueur pour le combat
    void PlayerAttack(Enemy enemy)
    {
        float damage = playerStats.CalculateDamage(); // Calcul avec critique
        enemy.TakeDamage(damage);
    }

    // Sauvegarder la progression
    void SaveGame()
    {
        var talentData = talentManager.GetSaveData();
        string json = JsonUtility.ToJson(talentData);
        PlayerPrefs.SetString("TalentSave", json);
    }

    // Charger la progression
    void LoadGame()
    {
        if (PlayerPrefs.HasKey("TalentSave"))
        {
            string json = PlayerPrefs.GetString("TalentSave");
            var talentData = JsonUtility.FromJson<TalentTreeManager.TalentSaveData>(json);
            talentManager.LoadSaveData(talentData);
        }
    }
}
```

---

## 🎮 Events disponibles

### PlayerStats Events
```csharp
playerStats.OnHealthChanged += (currentHp, maxHp) => {
    // Mettre à jour la barre de vie
};

playerStats.OnShieldChanged += (shield) => {
    // Mettre à jour la barre de bouclier
};

playerStats.OnStatsChanged += () => {
    // Rafraîchir l'UI des stats
};

playerStats.OnPlayerDied += () => {
    // Afficher l'écran de game over
};
```

### TalentTreeManager Events
```csharp
talentManager.OnTalentUnlocked += (talent, points) => {
    // Afficher une animation de déverrouillage
};

talentManager.OnTalentPointsChanged += (newPoints) => {
    // Mettre à jour le compteur de points
};

talentManager.OnLevelUp += (newLevel) => {
    // Afficher l'animation de level up
};

talentManager.OnTalentTreeReset += () => {
    // Rafraîchir l'UI de l'arbre
};
```

---

## 💡 Conseils et bonnes pratiques

### Structure de l'arbre
1. **Noeuds racines**: Talents accessibles dès le début (level 1, pas de parents)
2. **Branches**: Créez des chemins de spécialisation
3. **Synergie**: Faites en sorte que les talents se complètent

### Équilibrage
- Talents passifs: +10-20% par point pour les stats principales
- Talents actifs: Effets plus puissants mais coûtent plus de points
- Talents ultimes: Nécessitent plusieurs prérequis, coût élevé, effet majeur

### Performance
- Le système utilise des events pour minimiser les mises à jour inutiles
- Les calculs se font uniquement quand nécessaire
- La validation de l'arbre ne se fait qu'en mode éditeur

### Debug
Les managers ont des fonctions de debug accessibles via clic droit:
- **TalentTreeManager**: Add 10 Points, Level Up, Reset Talents
- **ExampleUsage**: Simulate Combat, Try Unlock First Talent, etc.

---

## 🔧 Personnalisation

### Ajouter un nouveau type de stat
1. Ajouter l'enum dans `StatModifier.cs` → `StatType`
2. Ajouter le champ dans `PlayerStats.cs`
3. Ajouter le modificateur correspondant
4. Ajouter le case dans `StatModifier.ApplyToPlayer()`

### Ajouter des effets spéciaux
Vous pouvez étendre `TalentNodeData` avec:
```csharp
public enum SpecialEffect
{
    None,
    LifeSteal,
    Thorns,
    DoubleShot,
    // etc.
}

public SpecialEffect specialEffect;
```

---

## 📝 Notes importantes

- **IDs uniques**: Chaque TalentNode doit avoir un ID unique (généré automatiquement)
- **Validation**: Le système valide automatiquement l'arbre au démarrage (mode éditeur)
- **Sauvegarde**: Les données sont sérialisables en JSON pour faciliter la sauvegarde
- **Namespace**: Tous les scripts utilisent le namespace `AstroSurvivor`

---

## 🐛 Troubleshooting

**"PlayerStats est null"**
→ Assurez-vous que PlayerStats et TalentTreeManager sont sur le même GameObject, ou assignez la référence manuellement

**"Noeud introuvable"**
→ Vérifiez que le TalentNode est bien ajouté à la liste du TalentTree

**"Impossible de débloquer"**
→ Vérifiez: niveau requis, parents débloqués, points disponibles

**"Dépendance cyclique"**
→ Un talent ne peut pas avoir comme parent un de ses propres enfants

---

Bon développement ! 🚀
