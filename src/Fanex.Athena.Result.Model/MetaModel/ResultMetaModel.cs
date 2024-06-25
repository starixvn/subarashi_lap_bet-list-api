// -----------------------------------------------------------------------
// <copyright file="ResultMetalModel.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Athena.Result.Model.MetaModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;

    /// <summary>
    /// ResultMetalModel class.
    /// </summary>
    [Serializable]
    public class ResultMetaModel
    {
        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        [Required]
        public bool IsOutRight { get; set; }

        [Required]
        public bool IsExportExcel { get; set; }

        [Required]
        public int SportId { get; set; }

        [Required]
        public int LeagueId { get; set; }

        [JsonIgnore]
        public int LicenseeId { get; set; }

        [JsonIgnore]
        public string LanguageCode { get; set; }

        [JsonIgnore]
        public int UserId { get; set; }

        [JsonIgnore]
        public bool IsHideSaba { get; set; }

        public IEnumerable<int> SpecialLeagues { get; set; }
    }
}