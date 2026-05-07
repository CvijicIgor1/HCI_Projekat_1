using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistem_za_upravljanje_sadrzajima.Modeli
{
    [Serializable]
    public class ArtistList
    {
        public ObservableCollection<Artist> Artists { get; set; } = new ObservableCollection<Artist>();
    }
}
