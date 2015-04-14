﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;

namespace Redwood.VS2015Extension.RwHtmlEditorExtensions.Classification
{
    [Export]
    public class ClassificationColorManager
    {
        private Dictionary<Type, ClassificationStyle> lightThemeStyleMap;
        private Dictionary<Type, ClassificationStyle> darkThemeStyleMap;

        public ClassificationColorManager()
        {
            lightThemeStyleMap = new Dictionary<Type, ClassificationStyle>()
            {
                { typeof(RwHtmlBindingBraceDefinition), new ClassificationStyle() { ForegroundColor = Colors.Blue, BackgroundColor = Colors.Yellow } },
                { typeof(RwHtmlBindingContentDefinition), new ClassificationStyle() { ForegroundColor = Colors.Blue, BackgroundColor = Color.FromRgb(244, 244, 244) } }
            };
            darkThemeStyleMap = new Dictionary<Type, ClassificationStyle>()
            {
                { typeof(RwHtmlBindingBraceDefinition), new ClassificationStyle() { ForegroundColor = Colors.Black, BackgroundColor = Color.FromRgb(255, 255, 179) } },
                { typeof(RwHtmlBindingContentDefinition), new ClassificationStyle() { ForegroundColor = Color.FromRgb(220, 220, 220), BackgroundColor = Color.FromRgb(80, 80, 80) } }
            };
        }

        public ClassificationStyle GetColor<T>() where T : ClassificationFormatDefinition
        {
            var theme = VisualStudioThemeManager.GetCurrentTheme();
            switch (theme)
            {
                case VisualStudioTheme.Dark:
                    return darkThemeStyleMap[typeof (T)];

                default:
                    return lightThemeStyleMap[typeof (T)];
            }
        }
    }
}