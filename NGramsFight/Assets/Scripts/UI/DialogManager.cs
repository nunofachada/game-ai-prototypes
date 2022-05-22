using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace AIUnityExample.NGramsFight.UI
{
    public class DialogManager : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI titleLabel;

        [SerializeField]
        private TextMeshProUGUI textLabel;

        [SerializeField]
        private Button buttonOK;

        private Action currentCallback;

        public void Dialog(string title, string text, Action callback)
        {
            currentCallback = callback;
            titleLabel.SetText(title);
            textLabel.SetText(text);
            gameObject.SetActive(true);
        }

        public void ClickOK()
        {
            currentCallback?.Invoke();
            currentCallback = null;
            gameObject.SetActive(false);
        }
    }
}