using UnityEngine;
using UnityEngine.UI;
using OMMG.Interaction;

namespace OMMG.Inventory
{
    /// <summary>
    /// 인벤토리 슬롯에 마우스를 올렸을 때 뜨는 툴팁.
    /// 씬에 미리 배치해두지 않고, 최초로 필요한 시점에 코드로 딱 한 번 생성한 뒤
    /// 이후에는 그 인스턴스를 계속 재사용하면서 위치와 표시 데이터만 바꾼다.
    /// </summary>
    public class ItemTooltip : MonoBehaviour
    {
        private static ItemTooltip instance;

        private RectTransform rectTransform;
        private Text nameText;
        private Text descText;
        private Text countText;

        /// <summary>anchor(슬롯의 RectTransform) 바로 위에 아이템 정보를 표시한다.</summary>
        public static void Show(RectTransform anchor, ItemData item, int count, Transform layerParent)
        {
            if (anchor == null || item == null) return;

            EnsureInstance(layerParent);
            instance.gameObject.SetActive(true);
            instance.SetData(item, count);
            instance.PositionAbove(anchor);
        }

        public static void Hide()
        {
            if (instance != null) instance.gameObject.SetActive(false);
        }

        private static void EnsureInstance(Transform layerParent)
        {
            if (instance != null) return;
            instance = Build(layerParent);
        }

        private static ItemTooltip Build(Transform layerParent)
        {
            var go = new GameObject("ItemTooltip(Runtime)");
            go.transform.SetParent(layerParent, false);

            var rt = go.AddComponent<RectTransform>();
            rt.pivot = new Vector2(0.5f, 0f); // 하단 중앙 피벗: position을 슬롯 윗변 중앙에 두면 툴팁이 그 위로 뜬다.
            rt.sizeDelta = new Vector2(240f, 10f);

            var bg = go.AddComponent<Image>();
            bg.color = new Color(0f, 0f, 0f, 0.85f);

            var group = go.AddComponent<CanvasGroup>();
            group.blocksRaycasts = false; // 툴팁 자체가 마우스 이벤트를 가로채 깜빡이지 않도록 함
            group.interactable = false;

            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(10, 10, 8, 8);
            layout.spacing = 3f;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childAlignment = TextAnchor.UpperLeft;

            var fitter = go.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var comp = go.AddComponent<ItemTooltip>();
            comp.rectTransform = rt;
            comp.nameText = CreateText(go.transform, "NameText", 18, FontStyle.Bold, new Color(1f, 0.92f, 0.6f));
            comp.descText = CreateText(go.transform, "DescText", 14, FontStyle.Normal, Color.white);
            comp.countText = CreateText(go.transform, "CountText", 13, FontStyle.Italic, new Color(0.8f, 0.8f, 0.8f));

            go.SetActive(false);
            return comp;
        }

        private static Text CreateText(Transform parent, string name, int fontSize, FontStyle style, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var text = go.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.fontStyle = style;
            text.color = color;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.alignment = TextAnchor.UpperLeft;

            var fitter = go.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.minWidth = 200f;
            layoutElement.preferredWidth = 220f;

            return text;
        }

        private void SetData(ItemData item, int count)
        {
            nameText.text = item.DisplayName;
            descText.text = string.IsNullOrEmpty(item.Description) ? "" : item.Description;
            descText.gameObject.SetActive(!string.IsNullOrEmpty(item.Description));
            countText.text = "보유 수량: " + count;
        }

        private void PositionAbove(RectTransform anchor)
        {
            var corners = new Vector3[4];
            anchor.GetWorldCorners(corners); // 0=bottom-left, 1=top-left, 2=top-right, 3=bottom-right

            Vector3 topCenter = (corners[1] + corners[2]) * 0.5f;
            rectTransform.position = topCenter + new Vector3(0f, 8f, 0f);
        }
    }
}
