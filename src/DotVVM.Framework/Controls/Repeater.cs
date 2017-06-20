using DotVVM.Framework.Binding;
using DotVVM.Framework.Binding.Expressions;
using DotVVM.Framework.Compilation.Javascript;
using DotVVM.Framework.Hosting;
using System.Linq;

namespace DotVVM.Framework.Controls
{
    /// <summary>
    /// Repeats a template for each item in the DataSource collection.
    /// </summary>
    [ControlMarkupOptions(AllowContent = false, DefaultContentProperty = nameof(ItemTemplate))]
    public class Repeater : ItemsControl
    {
        public static readonly DotvvmProperty EmptyDataTemplateProperty =
            DotvvmProperty.Register<ITemplate, Repeater>(t => t.EmptyDataTemplate, null);

        public static readonly DotvvmProperty ItemTemplateProperty =
            DotvvmProperty.Register<ITemplate, Repeater>(t => t.ItemTemplate, null);

        public static readonly DotvvmProperty RenderWrapperTagProperty =
            DotvvmProperty.Register<bool, Repeater>(t => t.RenderWrapperTag, true);

        public static readonly DotvvmProperty SeparatorTemplateProperty =
            DotvvmProperty.Register<ITemplate, Repeater>(t => t.SeparatorTemplate, null);

        public static readonly DotvvmProperty WrapperTagNameProperty =
            DotvvmProperty.Register<string, Repeater>(t => t.WrapperTagName, "div");

        private EmptyData emptyDataContainer;
        private int numberOfRows;

        public Repeater()
        {
            SetValue(Internal.IsNamingContainerProperty, true);
        }

        /// <summary>
        /// Gets or sets the template which will be displayed when the DataSource is empty.
        /// </summary>
        [MarkupOptions(AllowBinding = false, MappingMode = MappingMode.InnerElement)]
        public ITemplate EmptyDataTemplate
        {
            get { return (ITemplate)GetValue(EmptyDataTemplateProperty); }
            set { SetValue(EmptyDataTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the template for each Repeater item.
        /// </summary>
        [MarkupOptions(AllowBinding = false, MappingMode = MappingMode.InnerElement, Required = true)]
        [ControlPropertyBindingDataContextChange(nameof(DataSource))]
        [CollectionElementDataContextChange(1)]
        public ITemplate ItemTemplate
        {
            get { return (ITemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the control should render a wrapper element.
        /// </summary>
        [MarkupOptions(AllowBinding = false)]
        public bool RenderWrapperTag
        {
            get { return (bool)GetValue(RenderWrapperTagProperty); }
            set { SetValue(RenderWrapperTagProperty, value); }
        }

        /// <summary>
        /// Gets or sets the template containing the elements that separate items.
        /// </summary>
        [MarkupOptions(AllowBinding = false, MappingMode = MappingMode.InnerElement)]
        public ITemplate SeparatorTemplate
        {
            get { return (ITemplate)GetValue(SeparatorTemplateProperty); }
            set { SetValue(SeparatorTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the name of the tag that wraps the Repeater.
        /// </summary>
        [MarkupOptions(AllowBinding = false)]
        public string WrapperTagName
        {
            get { return (string)GetValue(WrapperTagNameProperty); }
            set { SetValue(WrapperTagNameProperty, value); }
        }

        protected override bool RendersHtmlTag => RenderWrapperTag;

        /// <summary>
        /// Occurs after the viewmodel is applied to the page and before the commands are executed.
        /// </summary>
        protected internal override void OnLoad(IDotvvmRequestContext context)
        {
            SetChildren(context);
            base.OnLoad(context);
        }

        /// <summary>
        /// Occurs after the page commands are executed.
        /// </summary>
        protected internal override void OnPreRender(IDotvvmRequestContext context)
        {
            SetChildren(context);     // TODO: we should handle observable collection operations to persist controlstate of controls inside the Repeater
            base.OnPreRender(context);
        }

        protected override void RenderBeginTag(IHtmlWriter writer, IDotvvmRequestContext context)
        {
            TagName = WrapperTagName;

            if (!RenderOnServer)
            {
                var javascriptDataSourceExpression = GetForeachDataBindExpression().GetKnockoutBindingExpression(this);

                if (RenderWrapperTag)
                {
                    writer.AddKnockoutForeachDataBind(javascriptDataSourceExpression);
                }
                else
                {
                    writer.WriteKnockoutForeachComment(javascriptDataSourceExpression);
                }
            }

            if (RenderWrapperTag)
            {
                base.RenderBeginTag(writer, context);
            }
        }

        /// <summary>
        /// Renders the contents inside the control begin and end tags.
        /// </summary>
        protected override void RenderContents(IHtmlWriter writer, IDotvvmRequestContext context)
        {
            if (RenderOnServer)
            {
                // render on server
                foreach (var child in Children.Except(new[] { emptyDataContainer }))
                {
                    child.Render(writer, context);
                }
            }
            else
            {
                // render on client
                if (SeparatorTemplate != null)
                {
                    writer.WriteKnockoutDataBindComment("if", "$index() > 0");
                    var separatorContainer = GetSeparator(context);
                    Children.Add(separatorContainer);
                    separatorContainer.Render(writer, context);
                    writer.WriteKnockoutDataBindEndComment();
                }

                var itemContainer = GetItem(context);
                Children.Add(itemContainer);
                itemContainer.Render(writer, context);
            }
        }

        protected override void RenderEndTag(IHtmlWriter writer, IDotvvmRequestContext context)
        {
            if (RenderWrapperTag)
            {
                base.RenderEndTag(writer, context);
            }

            if (!RenderOnServer && !RenderWrapperTag)
            {
                writer.WriteKnockoutDataBindEndComment();
            }

            emptyDataContainer?.Render(writer, context);
        }

        private DotvvmControl GetEmptyItem(IDotvvmRequestContext context)
        {
            var dataSourceBinding = GetDataSourceBinding();
            emptyDataContainer = new EmptyData();
            emptyDataContainer.SetValue(EmptyData.RenderWrapperTagProperty, GetValueRaw(RenderWrapperTagProperty));
            emptyDataContainer.SetValue(EmptyData.WrapperTagNameProperty, GetValueRaw(WrapperTagNameProperty));
            emptyDataContainer.SetBinding(DataSourceProperty, dataSourceBinding);
            EmptyDataTemplate.BuildContent(context, emptyDataContainer);
            return emptyDataContainer;
        }

        private DotvvmControl GetItem(IDotvvmRequestContext context, object item = null, int index = -1, IValueBinding itemBinding = null)
        {
            var container = new DataItemContainer();
            if (item == null)
            {
                SetUpClientItem(container);
            }
            else
            {
                SetUpServerItem(context, item, index, itemBinding, container);
            }

            ItemTemplate.BuildContent(context, container);
            return container;
        }

        private DotvvmControl GetSeparator(IDotvvmRequestContext context)
        {
            var placeholder = new PlaceHolder();
            SeparatorTemplate.BuildContent(context, placeholder);
            return placeholder;
        }

        /// <summary>
        /// Performs the data-binding and builds the controls inside the <see cref="Repeater"/>.
        /// </summary>
        private void SetChildren(IDotvvmRequestContext context)
        {
            Children.Clear();
            emptyDataContainer = null;

            if (DataSource != null)
            {
                var itemBinding = GetItemBinding();
                var index = 0;
                foreach (var item in GetIEnumerableFromDataSource())
                {
                    if (SeparatorTemplate != null && index > 0)
                    {
                        Children.Add(GetSeparator(context));
                    }
                    Children.Add(GetItem(context, item, index, itemBinding));
                    index++;
                }
            }

            if (EmptyDataTemplate != null)
            {
                Children.Add(GetEmptyItem(context));
            }
        }

        private void SetUpClientItem(DataItemContainer container)
        {
            container.DataContext = null;
            container.SetValue(Internal.PathFragmentProperty, GetPathFragmentExpression() + "/[$index]");
            container.SetValue(Internal.ClientIDFragmentProperty, GetValueRaw(Internal.CurrentIndexBindingProperty));
        }

        private void SetUpServerItem(IDotvvmRequestContext context, object item, int index, IValueBinding itemBinding, DataItemContainer container)
        {
            container.DataItemIndex = index;
            var bindingService = context.Configuration.ServiceLocator.GetService<BindingCompilationService>();
            container.SetBinding(DataContextProperty, ValueBindingExpression.CreateBinding(
                bindingService.WithoutInitialization(),
                j => item,
                itemBinding.KnockoutExpression.AssignParameters(p =>
                    p == JavascriptTranslator.CurrentIndexParameter ? new CodeParameterAssignment(index.ToString(), OperatorPrecedence.Max) :
                    default(CodeParameterAssignment))));
            container.SetValue(Internal.PathFragmentProperty, GetPathFragmentExpression() + "/[" + index + "]");
            container.ID = index.ToString();

        }
    }
}