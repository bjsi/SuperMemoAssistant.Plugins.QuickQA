using Anotar.Serilog;
using SuperMemoAssistant.Interop.SuperMemo.Content.Contents;
using SuperMemoAssistant.Interop.SuperMemo.Content.Models;
using SuperMemoAssistant.Interop.SuperMemo.Elements.Builders;
using SuperMemoAssistant.Interop.SuperMemo.Elements.Models;
using SuperMemoAssistant.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace SuperMemoAssistant.Plugins.QuickQA.UI
{
  /// <summary>
  /// Interaction logic for QuickQAWdw.xaml
  /// </summary>
  public partial class QuickQAWdw
  {

    public string Question { get; set; }
    public string Answer { get; set; }
    private static QuickQACfg Config => Svc<QuickQAPlugin>.Plugin.Config;

    public QuickQAWdw(string question, string answer)
    {
      this.Question = question ?? string.Empty;
      this.Answer = answer ?? string.Empty;

      InitializeComponent();

      if (!string.IsNullOrEmpty(question))
        FocusAnswerBox();
      else if (!string.IsNullOrEmpty(answer))
        FocusQuestionBox();
      else
        FocusQuestionBox();

      DataContext = this;
    }

    private void FocusQuestionBox()
    {
      QuestionTextBox.Dispatcher.BeginInvoke((Action)(() =>
              { 
                QuestionTextBox.Focus();
              }), DispatcherPriority.Render);
    }

    private void FocusAnswerBox()
    {
      AnswerTextBox.Dispatcher.BeginInvoke((Action)(() =>
              { 
                AnswerTextBox.Focus();
              }), DispatcherPriority.Render);

    }

    private void QuestionTextBox_KeyDown(object sender, KeyEventArgs e)
    {
      var key = e.Key;
      if (key == Key.Enter)
      {
        try
        {
          Hide();
          CreateSMElement();
        }
        finally
        {
          Close();
        }
      }
      else if (key == Key.Escape)
      {
        Close();
      }
    }

    private References GetReferences()
    {
      var htmlCtrl = ContentUtils.GetFirstHtmlControl();
      var html = htmlCtrl?.Text;
      if (!string.IsNullOrEmpty(html))
        return ReferenceParser.GetReferences(html);
      return null;
    }

    [LogToErrorOnException]
    private void CreateSMElement()
    {

      var refs = GetReferences();
      var contents = new List<ContentBase>();
      var parent = Svc.SM.UI.ElementWdw.CurrentElement;
      var question = Application.Current.Dispatcher.Invoke(() => QuestionTextBox.Text);
      var answer = Application.Current.Dispatcher.Invoke(() => AnswerTextBox.Text);
      contents.Add(new TextContent(true, question));
      contents.Add(new TextContent(true, answer, displayAt: AtFlags.NonQuestion));

      if (parent == null)
      {
        LogTo.Error("Failed to CreateSMElement because parent element was null");
        return;
      }

      bool success = Svc.SM.Registry.Element.Add(
        out _,
        ElemCreationFlags.None,
        new ElementBuilder(ElementType.Item, contents.ToArray())
          .WithParent(parent)
          .WithPriority(Config.DefaultPriority)
          .DoNotDisplay()
          .WithReference((_) => refs)
      );

      if (success)
      {
        LogTo.Debug("Successfully created SM Element");
      }
      else
      {
        LogTo.Error("Failed to CreateSMElement");
      }
    }

    private void MetroWindow_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
        Close();
    }

    private void OkBtn_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        Hide();
        CreateSMElement();
      }
      finally
      {
        Close();
      }
    }

    private void CancelBtn_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }

    private void AnswerTextBox_KeyDown(object sender, KeyEventArgs e)
    {

      var key = e.Key;
      if (key == Key.Enter)
      {
        try
        {
          Hide();
          CreateSMElement();
        }
        finally
        {
          Close();
        }
      }
      else if (key == Key.Escape)
      {
        Close();
      }
    }
  }
}
