using System;
using GDIPlusX.GDIPlus11.Effects;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SpotifyDesktop
{
    public partial class PlayerForm : Form
    {
        private string _title;
        private string _appFolder;
        private string _defaultCover;
        private const string _scrobblerApiKey = "f3a26c7c8b4c4306bc382557d5c04ad5";

        /// <summary>
        /// Creates the player form and starts the task that monitors Spotify for
        /// changes to its window title.
        /// </summary>
        public PlayerForm()
        {
            InitializeComponent();

            // Get the cover art folder location and filename
            _appFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _defaultCover = Path.Combine(_appFolder, "default_cover.png");

            // Turn the full file path into a URI for the PictureBox
            Uri uri = new Uri(_defaultCover);
            _defaultCover = uri.AbsoluteUri;

            // Load the default
            CoverArt.Load(_defaultCover);

            Task.Run(() =>
            {
                // Let the defaults load
                Thread.Sleep(1000);
                Process spotifyProcess = null;
                string spotifyTitle = string.Empty;

                // Look for the Spotify process and monitor its window title
                while (true)
                {
                    if (spotifyProcess == null)
                    {
                        var processes = Process.GetProcessesByName("spotify");
                        if (processes.Length < 1)
                        {
                            Thread.Sleep(5000);
                            continue;
                        }

                        spotifyProcess = processes[0];
                    }
                    else
                    {
                        spotifyProcess.Refresh();
                        if (spotifyProcess.HasExited)
                        {
                            spotifyProcess = null;
                            continue;
                        }

                        string title = spotifyProcess.MainWindowTitle;
                        if (title != spotifyTitle)
                        {
                            spotifyTitle = title;
                            SetLabel(title);
                        }
                        Thread.Sleep(5000);
                    }
                }
            });
        }

        /// <summary>
        /// Using the main window text passed in from the Spotify process, this method
        /// extracts the artist and song title.  It then uses the audio scrobbler web
        /// service to download details about the song so that we display accurate
        /// information.  We also use audio scrobbler's album cover art to update the
        /// album art displayed.
        /// </summary>
        /// <param name="label">The window label (main window text) of the Spotify
        /// process</param>
        public void SetLabel(string label)
        {
            if (InvokeRequired)
            {
                Invoke(new SetLabelDelegate(SetLabel), label);
            }
            else
            {
                // Get rid of the "Spotify - " prefix and then split out the artist
                // and song
                string noSpotify = label.Replace("Spotify - ", string.Empty);
                var parts = noSpotify.Split('–');
                if (parts.Length < 2)
                {
                    // Well.  Damn.  It appears we weren't passed any meaningful text,
                    // so go back to the defaults.
                    _title = "Song Title";
                    Artist.Text = "Artist";
                    CoverArt.Load(_defaultCover); 
                    return;
                }
                string artist = parts[0].Trim();
                string title = parts[1].Trim();
                _title = title;
                Artist.Text = artist;
                Invalidate();

                // Let's try to get the song details from Audio Scrobbler
                string url = "http://ws.audioscrobbler.com/2.0/?method=track.getinfo&api_key={0}&autocorrect=1&artist={1}&track={2}";
                artist = HttpUtility.UrlEncode(artist);
                title = HttpUtility.UrlEncode(title);
                url = string.Format(url, _scrobblerApiKey, artist, title);
                using (WebClient client = new WebClient())
                {
                    // Download the audioscrobbler XML for the given artist and song
                    string xml = client.DownloadString(url);
                    XDocument xmlDoc = XDocument.Load(new StringReader(xml));
                    var imgUrls = from img in xmlDoc.XPathSelectElements("//album/image")
                        where img.Attribute("size").Value == "large"
                        select img.Value;
                    string imgUrl = imgUrls.FirstOrDefault();
                    
                    // If we can't find an image URL, just use the default cover
                    if (imgUrl == null)
                    {
                        CoverArt.LoadAsync(_defaultCover); 
                    }
                    else
                    {
                        // Download the image data from audio scrobbler and dispose the previous
                        // cover art image (prevents a memory leak)
                        byte[] imageData = client.DownloadData(imgUrl);
                        using (Stream stream = new MemoryStream(imageData))
                        {
                            Image image = Image.FromStream(stream);
                            Image old = CoverArt.Image;
                            CoverArt.Image = image;
                            old.Dispose();
                        }
                    }
                }
            }
        }
        private delegate void SetLabelDelegate(string s);

        /// <summary>
        /// Event handler for the mouse move event.  Allows us to move the main window
        /// around the screen by clicking and dragging anywhere on the form's surface
        /// </summary>
        /// <param name="sender">The form (unused)</param>
        /// <param name="e">The mouse event, from which we can get the button state</param>
        private void MoveWithMouse(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !movingNow)
            {
                movingNow = true;
                lastX = e.X;
                lastY = e.Y;
            }
            else if (e.Button == MouseButtons.Left ||
                     (e.Button == MouseButtons.None && movingNow))
            {
                var difX = e.X - lastX;
                var difY = e.Y - lastY;
                var currentLocation = DesktopLocation;
                currentLocation.X += difX;
                currentLocation.Y += difY;
                DesktopLocation = currentLocation;
                Invalidate();

                if (e.Button == MouseButtons.None)
                {
                    movingNow = false;
                    Invalidate();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                Application.Exit();
            }
        }
        private bool movingNow;
        private int lastX;
        private int lastY;

        /// <summary>
        /// Repaints the form's surface.  This is used to draw text onto the form's
        /// transparent surface, showing the desktop background behind the text.  A
        /// blur effect is used to slightly darken the background before the text is
        /// drawn.
        /// </summary>
        /// <param name="sender">The form (unused)</param>
        /// <param name="e">The paint event details (unused - we repaint everything)</param>
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            using (var font = new Font("Segoe UI", 14, FontStyle.Bold))
            {
                using (var format = new StringFormat())
                {
                    var bm = new Bitmap(550, 60);
                    var effect = new BlurEffect(4.0f, false);
                    var g = Graphics.FromImage(bm);
                    var imgPoint = new Point(140, 70);
                    var screenPoint = PointToScreen(imgPoint);
                    g.CopyFromScreen(screenPoint, new Point(0,0), new Size(bm.Width, bm.Height));
                    
                    g.DrawString(_title, font, Brushes.Black, 10, 10);
                    bm.ApplyEffect(effect, Rectangle.Empty);
                    g.DrawString(_title, font, Brushes.White, 10, 10);

                    e.Graphics.DrawImage(bm, 140, 70, bm.Width, bm.Height);
                    g.Dispose();
                    bm.Dispose();
                }
            }
        }
    }
}
