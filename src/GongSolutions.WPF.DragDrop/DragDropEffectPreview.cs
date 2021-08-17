using System.Windows;

namespace GongSolutions.Wpf.DragDrop
{
    internal class DragDropEffectPreview : DragDropPreview
    {
        public DragDropEffectPreview(UIElement rootElement, UIElement previewElement, Point translation, DragDropEffects effects, string effectText, string destinationText)
            : base(rootElement, previewElement, translation, default)
        {
            this.Effects = effects;
            this.EffectText = effectText;
            this.DestinationText = destinationText;
        }

        public DragDropEffects Effects { get; set; }

        public string EffectText { get; set; }

        public string DestinationText { get; set; }
    }
}