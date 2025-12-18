using System.Collections.Generic;
using UnityEngine;

namespace AstroSurvivor
{
    /// <summary>
    /// Représente l'état d'un noeud de talent débloqué
    /// </summary>
    [System.Serializable]
    public class UnlockedTalent
    {
        public string nodeId;
        public int pointsInvested;
        public TalentNodeData nodeData;

        public UnlockedTalent(string id, TalentNodeData data, int points = 1)
        {
            nodeId = id;
            nodeData = data;
            pointsInvested = points;
        }
    }

    /// <summary>
    /// ScriptableObject représentant un arbre de talents complet
    /// </summary>
    [CreateAssetMenu(fileName = "NewTalentTree", menuName = "AstroSurvivor/Talent Tree")]
    public class TalentTree : ScriptableObject
    {
        [Header("Informations de l'Arbre")]
        [Tooltip("Nom de l'arbre de talents")]
        public string treeName;

        [Tooltip("Description de l'arbre")]
        [TextArea(2, 4)]
        public string treeDescription;

        [Tooltip("Icône de l'arbre")]
        public Sprite treeIcon;

        [Header("Noeuds")]
        [Tooltip("Tous les noeuds de talents disponibles dans cet arbre")]
        public List<TalentNodeData> talentNodes = new List<TalentNodeData>();

        [Header("Configuration")]
        [Tooltip("Points de talents gagnés à chaque niveau")]
        public int pointsPerLevel = 1;

        [Tooltip("Niveau maximum de l'arbre")]
        public int maxTreeLevel = 50;

        /// <summary>
        /// Récupère un noeud par son ID
        /// </summary>
        public TalentNodeData GetNodeById(string nodeId)
        {
            return talentNodes.Find(n => n.nodeId == nodeId);
        }

        /// <summary>
        /// Récupère tous les noeuds sans prérequis (racines de l'arbre)
        /// </summary>
        public List<TalentNodeData> GetRootNodes()
        {
            List<TalentNodeData> roots = new List<TalentNodeData>();
            
            foreach (var node in talentNodes)
            {
                if (node.parentNodeIds.Count == 0)
                {
                    roots.Add(node);
                }
            }

            return roots;
        }

        /// <summary>
        /// Récupère tous les enfants d'un noeud
        /// </summary>
        public List<TalentNodeData> GetChildNodes(string parentId)
        {
            List<TalentNodeData> children = new List<TalentNodeData>();

            foreach (var node in talentNodes)
            {
                if (node.parentNodeIds.Contains(parentId))
                {
                    children.Add(node);
                }
            }

            return children;
        }

        /// <summary>
        /// Valide la structure de l'arbre (pour debug)
        /// </summary>
        public bool ValidateTree()
        {
            bool isValid = true;

            // Vérifie que tous les IDs sont uniques
            HashSet<string> ids = new HashSet<string>();
            foreach (var node in talentNodes)
            {
                if (string.IsNullOrEmpty(node.nodeId))
                {
                    Debug.LogError($"Noeud '{node.talentName}' a un ID vide!");
                    isValid = false;
                }
                else if (ids.Contains(node.nodeId))
                {
                    Debug.LogError($"ID dupliqué détecté: {node.nodeId}");
                    isValid = false;
                }
                else
                {
                    ids.Add(node.nodeId);
                }
            }

            // Vérifie que tous les parents existent
            foreach (var node in talentNodes)
            {
                foreach (var parentId in node.parentNodeIds)
                {
                    if (!ids.Contains(parentId))
                    {
                        Debug.LogError($"Noeud '{node.talentName}' référence un parent inexistant: {parentId}");
                        isValid = false;
                    }
                }
            }

            // Vérifie qu'il n'y a pas de cycles
            foreach (var node in talentNodes)
            {
                if (HasCyclicDependency(node, new HashSet<string>()))
                {
                    Debug.LogError($"Dépendance cyclique détectée pour le noeud '{node.talentName}'");
                    isValid = false;
                }
            }

            return isValid;
        }

        private bool HasCyclicDependency(TalentNodeData node, HashSet<string> visitedNodes)
        {
            if (visitedNodes.Contains(node.nodeId))
            {
                return true; // Cycle détecté
            }

            visitedNodes.Add(node.nodeId);

            foreach (var parentId in node.parentNodeIds)
            {
                TalentNodeData parent = GetNodeById(parentId);
                if (parent != null && HasCyclicDependency(parent, new HashSet<string>(visitedNodes)))
                {
                    return true;
                }
            }

            return false;
        }

        private void OnValidate()
        {
            pointsPerLevel = Mathf.Max(1, pointsPerLevel);
            maxTreeLevel = Mathf.Max(1, maxTreeLevel);
        }
    }
}
