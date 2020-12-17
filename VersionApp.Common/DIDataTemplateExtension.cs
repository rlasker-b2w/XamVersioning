﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VersionApp.Common
{
	[ContentProperty(nameof(TypeName))]
	public sealed class DIDataTemplateExtension : IMarkupExtension<DataTemplate>
    {
		public string TypeName { get; set; }

		public DataTemplate ProvideValue(IServiceProvider serviceProvider)
		{
			if (serviceProvider == null)
				throw new ArgumentNullException(nameof(serviceProvider));
			if (!(serviceProvider.GetService(typeof(IXamlTypeResolver)) is IXamlTypeResolver typeResolver))
				throw new ArgumentException("No IXamlTypeResolver in IServiceProvider");
			if (string.IsNullOrEmpty(TypeName))
			{
				var li = (serviceProvider.GetService(typeof(IXmlLineInfoProvider)) is IXmlLineInfoProvider lip) ? lip.XmlLineInfo : new XmlLineInfo();
				throw new XamlParseException("TypeName isn't set.", li);
			}

            if (typeResolver.TryResolve(TypeName, out var type))
            {
                return new DataTemplate(() => ServiceLocator.GetProvider().GetService(type));
            }

            var lineInfo = (serviceProvider.GetService(typeof(IXmlLineInfoProvider)) is IXmlLineInfoProvider lineInfoProvider) ? lineInfoProvider.XmlLineInfo : new XmlLineInfo();
			throw new XamlParseException($"DataTemplateExtension: Could not locate type for {TypeName}.", lineInfo);
		}

		object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => (this as IMarkupExtension<DataTemplate>).ProvideValue(serviceProvider);
	}
}
