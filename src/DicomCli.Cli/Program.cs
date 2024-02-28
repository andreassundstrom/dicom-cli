using FellowOakDicom;
using FellowOakDicom.Imaging;
using FellowOakDicom.IO.Buffer;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

GenerateData(out DicomDataset ds);
SetPixelData(ds);
StoreDicomData(ds);

static void SetPixelData(DicomDataset ds)
{
    const int width = 500;
    const int height = 500;

    byte[] pixelDataBytes = new byte[width * height * 3];
    using (Image<Rgb24> img = new Image<Rgb24>(width, height))
    {
        FontCollection fonts = new FontCollection();
        FontFamily fontFamily = fonts.Add(@"fonts\3270NerdFont-Regular.ttf");
        Font font = fontFamily.CreateFont(12);

        img.Mutate(i => i.Fill(Color.Azure));
        img.Mutate(x => x.Fill(Color.Gold, new Star(x: 100.0f, 100.0f, prongs: 5, innerRadii: 10.0f, outerRadii: 30.0f)));
        img.Mutate(x => x.DrawText("Hello world!", font, Color.BurlyWood, new PointF(50f, 50f)));

        img.CopyPixelDataTo(pixelDataBytes);

        ds.Add(DicomTag.BitsAllocated, (ushort)8);
        ds.Add(DicomTag.PhotometricInterpretation, PhotometricInterpretation.Rgb.Value);
        ds.Add(DicomTag.Rows, (ushort)img.Height);
        ds.Add(DicomTag.Columns, (ushort)img.Width);
        DicomPixelData pixelData = DicomPixelData.Create(ds, true);
        pixelData.BitsStored = 8;
        pixelData.SamplesPerPixel = 3;
        pixelData.HighBit = 7;
        pixelData.PixelRepresentation = 0;
        pixelData.PlanarConfiguration = 0;
        pixelData.AddFrame(new MemoryByteBuffer(pixelDataBytes));
    }
}

static void GenerateData(out DicomDataset ds)
{
    ds = new DicomDataset();

    ds.Add(DicomTag.SOPInstanceUID, DicomUID.Generate());
    ds.Add(DicomTag.SOPClassUID, DicomUID.SecondaryCaptureImageStorage);
    ds.Add(DicomTag.SeriesInstanceUID, DicomUID.Generate());
    ds.Add(DicomTag.StudyInstanceUID, DicomUID.Generate());
    ds.Add(DicomTag.PatientID, "991212121212");
    ds.Add(DicomTag.PatientName, "Test Testsson");
    ds.Add(DicomTag.PatientBirthDate, "19001212");
    ds.Add(DicomTag.Modality, "CT");
}

static void StoreDicomData(DicomDataset ds)
{
    DicomFile file = new DicomFile(ds);
    file.Save("Testfile.dcm");
}