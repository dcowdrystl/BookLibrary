using System.Collections.Generic;

namespace BookLibrary.Models
{
    /*    public class GoogleBookSearchResult
        {
            public string Kind { get; set; }
            public int TotalItems { get; set; }
            public List<GoogleBookResult> Items { get; set; }

            public GoogleBookSearchResult()
            {
            }
        }

        public class GoogleBookResult
        {
            public string Id { get; set; }
            public VolumeInfo VolumeInfo { get; set; }
            public SearchInfo SearchInfo { get; set; }

            public GoogleBookResult()
            {
            }
        }*/

    /*public class VolumeInfo
    {
        public string Title { get; set; }
        public List<string> Authors { get; set; }
        public string Publisher { get; set; }
        public string PublishedDate { get; set; }

        public int PageCount { get; set; }
        public ImageLinks ImageLinks { get; set; }


        public VolumeInfo()
        {
        }
    }
    public class SearchInfo
    {
        public string TextSnippet { get; set; }
        public SearchInfo()
        {
        }
    }
    public class ImageLinks
    {
        public string Thumbnail { get; set; }

        public ImageLinks()
        {
        }
    }*/
    public class AccessInfo
    {
        public string Country { get; set; }
        public string Viewability { get; set; }
        public bool Embeddable { get; set; }
        public bool PublicDomain { get; set; }
        public string TextToSpeechPermission { get; set; }
        public Epub Epub { get; set; }
        public Pdf Pdf { get; set; }
        public string WebReaderLink { get; set; }
        public string AccessViewStatus { get; set; }
        public bool QuoteSharingAllowed { get; set; }
    }

    public class Epub
    {
        public bool isAvailable { get; set; }
        public string acsTokenLink { get; set; }
    }

    public class ImageLinks
    {
        public string SmallThumbnail { get; set; }
        public string Thumbnail { get; set; }
    }

    public class IndustryIdentifier
    {
        public string type { get; set; }
        public string identifier { get; set; }
    }

    public class GoogleBookResult
    {
        public string Kind { get; set; }
        public string Id { get; set; }
        public string etag { get; set; }
        public string selfLink { get; set; }
        public VolumeInfo VolumeInfo { get; set; }
        public SaleInfo saleInfo { get; set; }
        public AccessInfo accessInfo { get; set; }
        public SearchInfo SearchInfo { get; set; }
    }

    public class ListPrice
    {
        public double amount { get; set; }
        public string currencyCode { get; set; }
        public int amountInMicros { get; set; }
    }

    public class Offer
    {
        public int finskyOfferType { get; set; }
        public ListPrice listPrice { get; set; }
        public RetailPrice retailPrice { get; set; }
        public bool giftable { get; set; }
    }

    public class PanelizationSummary
    {
        public bool containsEpubBubbles { get; set; }
        public bool containsImageBubbles { get; set; }
        public string epubBubbleVersion { get; set; }
        public string imageBubbleVersion { get; set; }
    }

    public class Pdf
    {
        public bool isAvailable { get; set; }
        public string acsTokenLink { get; set; }
    }

    public class ReadingModes
    {
        public bool text { get; set; }
        public bool image { get; set; }
    }

    public class RetailPrice
    {
        public double amount { get; set; }
        public string currencyCode { get; set; }
        public int amountInMicros { get; set; }
    }

    public class GoogleBookSearchResult
    {
        public string Kind { get; set; }
        public int TotalItems { get; set; }
        public List<GoogleBookResult> Items { get; set; }
    }

    public class SaleInfo
    {
        public string country { get; set; }
        public string saleability { get; set; }
        public bool isEbook { get; set; }
        public ListPrice listPrice { get; set; }
        public RetailPrice retailPrice { get; set; }
        public string buyLink { get; set; }
        public List<Offer> offers { get; set; }
    }

    public class SearchInfo
    {
        public string TextSnippet { get; set; }
    }

    public class SeriesInfo
    {
        public string kind { get; set; }
        public string bookDisplayNumber { get; set; }
        public List<VolumeSeries> volumeSeries { get; set; }
    }

    public class VolumeInfo
    {
        public string Title { get; set; }
        public List<string> Authors { get; set; }
        public string Publisher { get; set; }
        public string PublishedDate { get; set; }
        public string Description { get; set; }
        public List<IndustryIdentifier> IndustryIdentifiers { get; set; }
        public ReadingModes ReadingModes { get; set; }
        public int PageCount { get; set; }
        public string PrintType { get; set; }
        public List<string> Categories { get; set; }
        public int AverageRating { get; set; }
        public int RatingsCount { get; set; }
        public string maturityRating { get; set; }
        public bool allowAnonLogging { get; set; }
        public string contentVersion { get; set; }
        public PanelizationSummary panelizationSummary { get; set; }
        public ImageLinks ImageLinks { get; set; }
        public string language { get; set; }
        public string previewLink { get; set; }
        public string InfoLink { get; set; }
        public string canonicalVolumeLink { get; set; }
        public string subtitle { get; set; }
        public SeriesInfo seriesInfo { get; set; }
    }

    public class VolumeSeries
    {
        public string seriesId { get; set; }
        public string seriesBookType { get; set; }
        public int orderNumber { get; set; }
    }

}

