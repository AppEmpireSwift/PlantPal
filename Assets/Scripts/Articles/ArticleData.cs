using System;
using UnityEngine;

namespace Articles
{
    [Serializable]
    public class ArticleData
    {
        public Sprite Sprite;
        public string Title;
        public string Description;
        public bool IsFavourite;
    }
}