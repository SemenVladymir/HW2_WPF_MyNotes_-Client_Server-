using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HW2_WPF_MyNotes__Client_Server_.Models
{
    public class MyNote
    {
        public int id { get; set; }
        public DateTime noteDateTime { get; set; }
        public string? description { get; set; }
        public string shortDescription { get; set; }

        public MyNote()
        {
            id = 0;
            noteDateTime = DateTime.Now;
            description = string.Empty;
            shortDescription = string.Empty;
        }

        public MyNote(DateTime noteDateTime, string? description, string shortDescription)
        {
            this.noteDateTime = noteDateTime;
            this.description = description;
            this.shortDescription = shortDescription;
        }
    }
}
