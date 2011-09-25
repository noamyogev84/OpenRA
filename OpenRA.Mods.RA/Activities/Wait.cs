#region Copyright & License Information
/*
 * Copyright 2007-2011 The OpenRA Developers (see AUTHORS)
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation. For more information,
 * see COPYING.
 */
#endregion

using OpenRA.Traits;
using OpenRA.Traits.Activities;

namespace OpenRA.Mods.RA.Activities
{
	public class Wait : Activity
	{
		int remainingTicks;
		bool interruptable = true;

		public Wait(int period) { remainingTicks = period; }
		public Wait(int period, bool interruptable)
		{
			remainingTicks = period;
			this.interruptable = interruptable;
		}

		public override Activity Tick(Actor self)
		{
			return (remainingTicks-- == 0) ? NextActivity: this;
		}

		public override void Cancel( Actor self )
		{
			if( !interruptable )
				return;

			remainingTicks = 0;
			base.Cancel(self);
		}
	}
}
