using System;
using System.Windows.Media.Animation;
using System.Windows;

namespace Pollux.Behavior
{
	public class GridLengthAnimation : AnimationTimeline
	{
		public static readonly DependencyProperty FromProperty;
		public static readonly DependencyProperty ToProperty;

        public static readonly DependencyProperty EasingFunctionProperty;
      

		static GridLengthAnimation()
		{
            Type typeofThis = typeof(GridLengthAnimation);
			FromProperty = DependencyProperty.Register("From", typeof(GridLength?), typeof(GridLengthAnimation));
			ToProperty = DependencyProperty.Register("To", typeof(GridLength?), typeof(GridLengthAnimation));
            EasingFunctionProperty = DependencyProperty.Register("EasingFunction",typeof(IEasingFunction),typeofThis);
		}
        //public new GridLengthAnimation Clone()
        //{
        //    return (GridLengthAnimation)base.Clone();
        //}

		protected override Freezable CreateInstanceCore()
		{
			return new GridLengthAnimation();
		}

		public override Type TargetPropertyType
		{
			get { return typeof(GridLength); }
		}
        public IEasingFunction EasingFunction
        {
            get
            {
                return (IEasingFunction)GetValue(GridLengthAnimation.EasingFunctionProperty);
            }
            set
            {
                SetValue(GridLengthAnimation.EasingFunctionProperty, value);
            }
        }
		public GridLength? From
		{
			get
			{
				return (GridLength?)GetValue(GridLengthAnimation.FromProperty);
			}
			set
			{
				SetValue(GridLengthAnimation.FromProperty, value);
			}
		}

		public GridLength? To
		{
			get
			{
				return (GridLength?)GetValue(GridLengthAnimation.ToProperty);
			}
			set
			{
				SetValue(GridLengthAnimation.ToProperty, value);
			}
		}
        public override sealed object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
        {
            // Verify that object arguments are non-null since we are a value type
            if (defaultOriginValue == null)
            {
                throw new ArgumentNullException("defaultOriginValue");
            }
            if (defaultDestinationValue == null)
            {
                throw new ArgumentNullException("defaultDestinationValue");
            }
            return GetCurrentValue((GridLength)defaultOriginValue, (GridLength)defaultDestinationValue, animationClock);
        }

        public GridLength GetCurrentValue(GridLength defaultOriginValue, GridLength defaultDestinationValue, AnimationClock animationClock)
		{
            VerifyAccess();

            if (animationClock == null)
            {
                throw new ArgumentNullException("animationClock");
            }

            // We check for null above but presharp doesn't notice so we suppress the 
            // warning here.
            #pragma warning disable 6506
            if (animationClock.CurrentState == ClockState.Stopped)
            {
                return defaultDestinationValue;
            }

            return GetCurrentValueCore(defaultOriginValue, defaultDestinationValue, animationClock);

		}

        public  GridLength GetCurrentValueCore(GridLength defaultOriginValue, GridLength defaultDestinationValue, AnimationClock animationClock)
        {
            double progress = animationClock.CurrentProgress.Value;

            IEasingFunction easingFunction = EasingFunction;
            if (easingFunction != null)
            {
                progress = easingFunction.Ease(progress);
            }

            double from = defaultOriginValue.Value;
            
            if (From.HasValue)
                from = From.Value.Value;

            double to = defaultDestinationValue.Value;
            
            if (To.HasValue)
                to = To.Value.Value;


            if (from > to)
            {
                //return new GridLength((1 - animationClock.CurrentProgress.Value) * (from - to) + to, GetGridUnitType());
                return new GridLength((1 - progress) * (from - to) + to, GetGridUnitType());
                //return new GridLength((1 - animationClock.CurrentProgress.Value) * (fromValue - toValue) + toValue,GridUnitType.Star);
            }
            else
            {
                //return new GridLength(from + (to - from) * animationClock.CurrentProgress.Value, GetGridUnitType());
                return new GridLength(from + (to - from) * progress,GetGridUnitType());
                //return new GridLength((animationClock.CurrentProgress.Value) * (toValue - fromValue) + fromValue,GridUnitType.Star);
            }
        }

        public GridUnitType GetGridUnitType()
        {
            return this.To.Value.IsStar ? GridUnitType.Star : this.To.Value.IsAuto ? GridUnitType.Auto : GridUnitType.Pixel;
        }


	}
}
