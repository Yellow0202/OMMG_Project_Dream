using System.Collections.Generic;
using UnityEngine;

namespace OMMG.Interaction
{
    /// <summary>
    /// 대화 선택지 목록 UI 싱글턴. DialogueRunner가 넘겨준 선택지들을 줄 단위로 그린다.
    /// </summary>
    public class DialogueChoiceUI : MonoBehaviour
    {
        public static DialogueChoiceUI Instance { get; private set; }

        [SerializeField] private GameObject panelRoot;
        [SerializeField] private RectTransform rowContainer;
        [SerializeField] private DialogueChoiceRow rowTemplate;

        private readonly List<DialogueChoiceRow> spawnedRows = new List<DialogueChoiceRow>();

        private void Awake()
        {
            Instance = this;
            if (panelRoot != null) panelRoot.SetActive(false);
        }

        public void Show(List<DialogueChoice> choices)
        {
            Clear();
            if (panelRoot != null) panelRoot.SetActive(true);

            if (rowTemplate == null || rowContainer == null || choices == null) return;

            for (int i = 0; i < choices.Count; i++)
            {
                var rowGO = Instantiate(rowTemplate.gameObject, rowContainer);
                rowGO.SetActive(true);

                var row = rowGO.GetComponent<DialogueChoiceRow>();
                row.Bind(i, choices[i].Text);
                spawnedRows.Add(row);
            }
        }

        public void Hide()
        {
            Clear();
            if (panelRoot != null) panelRoot.SetActive(false);
        }

        private void Clear()
        {
            for (int i = 0; i < spawnedRows.Count; i++)
            {
                if (spawnedRows[i] != null) Destroy(spawnedRows[i].gameObject);
            }
            spawnedRows.Clear();
        }
    }
}
