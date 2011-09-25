#region Copyright & License Information
/*
 * Copyright 2007-2011 The OpenRA Developers (see AUTHORS)
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation. For more information,
 * see COPYING.
 */
#endregion

using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenRA.FileFormats;
using OpenRA.Widgets;

namespace OpenRA
{
	public class WidgetLoader
	{
		Dictionary<string, MiniYamlNode> widgets = new Dictionary<string, MiniYamlNode>();

		public WidgetLoader( ModData modData )
		{
			foreach( var file in modData.Manifest.ChromeLayout.Select( a => MiniYaml.FromFile( a ) ) )
				foreach( var w in file )
				{
					var key = w.Key.Substring( w.Key.IndexOf( '@' ) + 1 );
					if (widgets.ContainsKey(key))
						throw new InvalidDataException("Widget has duplicate Key `{0}`".F(w.Key));
					widgets.Add( key, w );
				}
		}

		public Widget LoadWidget( WidgetArgs args, Widget parent, string w )
		{
			MiniYamlNode ret;
			if (!widgets.TryGetValue(w, out ret))
				throw new InvalidDataException("Cannot find widget with Id `{0}`".F(w));

			return LoadWidget( args, parent, ret );
		}

		public Widget LoadWidget( WidgetArgs args, Widget parent, MiniYamlNode node)
		{
			var widget = NewWidget(node.Key, args);

			if (parent != null)
				parent.AddChild( widget );

			foreach (var child in node.Value.Nodes)
				if (child.Key != "Children")
					FieldLoader.LoadField(widget, child.Key, child.Value.Value);

			widget.Initialize(args);

			foreach (var child in node.Value.Nodes)
				if (child.Key == "Children")
					foreach (var c in child.Value.Nodes)
						LoadWidget( args, widget, c);

			widget.PostInit( args );
			return widget;
		}

		Widget NewWidget(string widgetType, WidgetArgs args)
		{
			widgetType = widgetType.Split('@')[0];
			return Game.modData.ObjectCreator.CreateObject<Widget>(widgetType + "Widget", args);
		}
	}
}