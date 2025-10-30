using System.ComponentModel.DataAnnotations;
using NetEscapades.EnumGenerators;

namespace MMKiwi.CtaTracker.Model;

[EnumExtensions]
public enum TimeResolution
{
    [Display(Name = "m")] Minutes,
    [Display(Name = "s")] Seconds
}