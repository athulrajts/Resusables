using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace KEI.UI.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for Wizard.xaml
    /// </summary>
    public partial class Wizard : UserControl
    {
        public Wizard()
        {
            InitializeComponent();
        }

        public IWizardStep CurrentStep
        {
            get { return (IWizardStep)GetValue(CurrentStepProperty); }
            set { SetValue(CurrentStepProperty, value); }
        }

        public static readonly DependencyProperty CurrentStepProperty =
            DependencyProperty.Register("CurrentStep", typeof(IWizardStep), typeof(Wizard), new PropertyMetadata(null, OnCurrentStepChanged));

        private static void OnCurrentStepChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var wiz = d as Wizard;

            if (e.OldValue is not null)
            {
                if (wiz.WizardPresenter.Content is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            if (e.NewValue is IWizardStep step &&
                step.ViewType is not null)
            {
                wiz.WizardPresenter.Content = Activator.CreateInstance(step.ViewType);
            }

        }

        public WizardStepCollection Steps
        {
            get { return (WizardStepCollection)GetValue(StepsProperty); }
            set { SetValue(StepsProperty, value); }
        }

        public static readonly DependencyProperty StepsProperty =
            DependencyProperty.Register("Steps", typeof(WizardStepCollection), typeof(Wizard), new PropertyMetadata(null, OnStepsChanged));

        private static void OnStepsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Wizard wiz = d as Wizard;

            if (e.NewValue is WizardStepCollection steps)
            {
                wiz.CurrentStep = steps.FirstOrDefault();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            int currentStepIndex = Steps.IndexOf(CurrentStep);

            if (currentStepIndex < Steps.Count - 1)
            {
                CurrentStep = Steps[currentStepIndex + 1];
            }
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            int currentStepIndex = Steps.IndexOf(CurrentStep);

            if (currentStepIndex > 0)
            {
                CurrentStep = Steps[currentStepIndex - 1];
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public class WizardStepCollection : List<IWizardStep> { }
}
