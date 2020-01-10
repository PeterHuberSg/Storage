using System;
using System.Collections.Generic;
using System.Text;

namespace Storage {


  /// <summary>
  /// Exensions used to limit the value of .Net value types like decimals or DateTime to a smaller data range, which takes
  /// less space to store in a CSV file. The definitions here helps also that the generated code can be the same for
  /// nullable and not nullable types.
  /// </summary>
  public static class Rounding {

    #region DateTime and TimeSpan
    //      ---------------------

    /// <summary>
    /// TimeSpan of 1 day
    /// </summary>
    public static readonly TimeSpan Days = TimeSpan.FromDays(1);

    /// <summary>
    /// TimeSpan of 1 hour
    /// </summary>
    public static readonly TimeSpan Hours = TimeSpan.FromHours(1);

    /// <summary>
    /// TimeSpan of 1 minute
    /// </summary>
    public static readonly TimeSpan Minutes = TimeSpan.FromMinutes(1);

    /// <summary>
    /// TimeSpan of 1 second
    /// </summary>
    public static readonly TimeSpan Seconds = TimeSpan.FromSeconds(1);


    /// <summary>
    /// Rounds dateTime to nearest timeRange.
    /// </summary>
    public static DateTime Round(this DateTime dateTime, TimeSpan timeRange) {
      long ticks = (dateTime.Ticks + (timeRange.Ticks / 2) + 1)/ timeRange.Ticks;
      return new DateTime(ticks * timeRange.Ticks, dateTime.Kind);
    }


    /// <summary>
    /// Rounds dateTime to nearest timeRange.
    /// </summary>
    public static DateTime? Round(this DateTime? dateTime, TimeSpan timeRange) {
      if (dateTime is null) return null;

      long ticks = (dateTime.Value.Ticks + (timeRange.Ticks / 2) + 1)/ timeRange.Ticks;
      return new DateTime(ticks * timeRange.Ticks, dateTime.Value.Kind);
    }


    /// <summary>
    /// Rounds dateTime down to nearest timeRange.
    /// </summary>
    public static DateTime Floor(this DateTime dateTime, TimeSpan timeRange) {
      long ticks = (dateTime.Ticks / timeRange.Ticks);
      return new DateTime(ticks * timeRange.Ticks, dateTime.Kind);
    }


    /// <summary>
    /// Rounds dateTime down to nearest timeRange.
    /// </summary>
    public static DateTime? Floor(this DateTime? dateTime, TimeSpan timeRange) {
      if (dateTime is null) return null;

      long ticks = (dateTime.Value.Ticks / timeRange.Ticks);
      return new DateTime(ticks * timeRange.Ticks, dateTime.Value.Kind);
    }


    /// <summary>
    /// Rounds dateTime up to nearest timeRange.
    /// </summary>
    public static DateTime Ceil(this DateTime dateTime, TimeSpan timeRange) {
      long ticks = (dateTime.Ticks + timeRange.Ticks - 1) / timeRange.Ticks;
      return new DateTime(ticks * timeRange.Ticks, dateTime.Kind);
    }


    /// <summary>
    /// Rounds dateTime up to nearest timeRange.
    /// </summary>
    public static DateTime? Ceil(this DateTime? dateTime, TimeSpan timeRange) {
      if (dateTime is null) return null;

      long ticks = (dateTime.Value.Ticks + timeRange.Ticks - 1) / timeRange.Ticks;
      return new DateTime(ticks * timeRange.Ticks, dateTime.Value.Kind);
    }


    /// <summary>
    /// Rounds timeSpan to nearest timeRange.
    /// </summary>
    public static TimeSpan Round(this TimeSpan timeSpan, TimeSpan timeRange) {
      long ticks = (timeSpan.Ticks + (timeRange.Ticks / 2) + 1)/ timeRange.Ticks;
      return new TimeSpan(ticks * timeRange.Ticks);
    }


    /// <summary>
    /// Rounds timeSpan to nearest timeRange.
    /// </summary>
    public static TimeSpan? Round(this TimeSpan? timeSpan, TimeSpan timeRange) {
      if (timeSpan is null) return null;

      long ticks = (timeSpan.Value.Ticks + (timeRange.Ticks / 2) + 1)/ timeRange.Ticks;
      return new TimeSpan(ticks * timeRange.Ticks);
    }
    #endregion


    #region decimal
    //      -------

    /// <summary>
    /// Returns number with at most digitsNumber of digits after decimal point 
    /// </summary>
    public static decimal Round(this decimal number, int digitsNumber) {
      return Math.Round(number, digitsNumber);
    }


    /// <summary>
    /// Returns number with at most digitsNumber of digits after decimal point 
    /// </summary>
    public static decimal? Round(this decimal? number, int digitsNumber) {
      if (number is null) return null;

      return Math.Round(number.Value, digitsNumber);
    }
    #endregion
  }

}