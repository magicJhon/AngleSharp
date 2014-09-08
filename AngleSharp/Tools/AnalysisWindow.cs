﻿namespace AngleSharp.Tools
{
    using AngleSharp.DOM;
    using AngleSharp.DOM.Collections;
    using AngleSharp.DOM.Css;
    using AngleSharp.DOM.Events;
    using AngleSharp.DOM.Html;
    using AngleSharp.DOM.Navigator;
    using System;

    /// <summary>
    /// Represents a sample browsing Window implementation for
    /// automated tests, analysis and as a useful playground.
    /// </summary>
    public class AnalysisWindow : EventTarget, IWindow, IWindowProxy
    {
        #region ctor

        /// <summary>
        /// Creates a new analysis windows context without any starting
        /// document. The height and width properties are also not set.
        /// </summary>
        public AnalysisWindow()
        {
        }

        /// <summary>
        /// Creates a new analysis window starting with the given document.
        /// </summary>
        /// <param name="document">The document to use.</param>
        public AnalysisWindow(IDocument document)
        {
            Document = document;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a reference to the document that the window contains.
        /// </summary>
        public IDocument Document
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the window.
        /// </summary>
        public String Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the height of the outside of the browser window.
        /// </summary>
        public Int32 OuterHeight
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the width of the outside of the browser window.
        /// </summary>
        public Int32 OuterWidth
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the horizontal distance of the left border of the user's
        /// browser from the left side of the screen.
        /// </summary>
        public Int32 ScreenX
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the vertical distance of the top border of the user's
        /// browser from the top side of the screen.
        /// </summary>
        public Int32 ScreenY
        {
            get;
            set;
        }

        public ILocation Location
        {
            get { return Document.Location; }
        }

        public String Status
        {
            get;
            set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gives the values of all the CSS properties of an element after
        /// applying the active stylesheets and resolving any basic computation
        /// those values may contain.
        /// </summary>
        /// <param name="element">The element to compute the styles for.</param>
        /// <param name="pseudo">The optional pseudo selector to use.</param>
        /// <returns>The style declaration describing the element.</returns>
        public ICssStyleDeclaration GetComputedStyle(IElement element, String pseudo = null)
        {
            var document = Document as Document;

            if (document == null)
                throw new ArgumentException("A valid HTML document is required for computing the style of an element.");

            // if pseudo is :before OR ::before then use the corresponding pseudo-element
            // else if pseudo is :after OR ::after then use the corresponding pseudo-element

            var bag = new CssPropertyBag();

            foreach (var stylesheet in document.StyleSheets)
            {
                var sheet = stylesheet as CSSStyleSheet;

                if (sheet != null && !stylesheet.IsDisabled && ((MediaList)stylesheet.Media).Validate(this))//TODO remove cast ASAP
                {
                    var rules = sheet.Rules as CSSRuleList;

                    foreach (var rule in rules.List)
                        rule.ComputeStyle(bag, this, element);
                }
            }

            var htmlElement = element as IHtmlElement;

            if (htmlElement != null)
                bag.ExtendWith(htmlElement.Style, Priority.Inline);

            bag.InheritFrom(element, this);
            return new CSSStyleDeclaration(bag);
        }

        #endregion

        #region Browsing context

        /// <summary>
        /// Gets the proxy to the current browsing context.
        /// </summary>
        public IWindowProxy Proxy
        {
            get { return this; }
        }

        /// <summary>
        /// Gets the user-agent information.
        /// </summary>
        public INavigator Navigator
        {
            get { return null; }
        }

        #endregion
    }
}
