using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// ========== CÂU 1: Giao diện và Lớp Trừu tượng ==========

// 1. Tạo interface IMedia
public interface IMedia
{
    string Creator { get; set; }
    DateTime ReleaseDate { get; }
    int Duration { get; }
}

// 2. Tạo abstract class MediaItem
public abstract class MediaItem
{
    // Trường private: _code (int), _name (string)
    private int _code;
    private string _name;

    // Property: Code (read only)
    public int Code
    {
        get { return _code; }
    }

    // Property: Name (read/write, nếu < 3 ký tự -> Exception)
    public string Name
    {
        get { return _name; }
        set
        {
            if (value.Length < 3)
            {
                throw new Exception("Tên (Name) phải có ít nhất 3 ký tự.");
            }
            _name = value;
        }
    }

    // Constructor 1: (int code) -> gán _code = code, _name = "Unknown"
    public MediaItem(int code)
    {
        _code = code;
        _name = "Unknown";
    }

    // Constructor 2: (int code, string name) -> gán đầy đủ
    public MediaItem(int code, string name)
    {
        _code = code;
        // Gán qua Property để kiểm tra Name
        Name = name;
    }

    // Abstract method: void DisplayInfo()
    public abstract void DisplayInfo();
}

// ========== CÂU 2: Lớp Kế thừa Movie ==========

public class Movie : MediaItem, IMedia
{
    // Trường private (của IMedia): _creator, _releaseDate, _duration
    private string _creator;
    private DateTime _releaseDate;
    private int _duration; // đơn vị phút (mins)

    // Thuộc tính công khai của IMedia:
    public string Creator
    {
        get { return _creator; }
        set { _creator = value; }
    }

    public DateTime ReleaseDate
    {
        get { return _releaseDate; }
    }

    public int Duration
    {
        get { return _duration; }
    }

    // Constructor 1: (int code, string name) -> gọi base, "N/A", DateTime.Now, duration=0
    public Movie(int code, string name)
        : base(code, name) // Gọi constructor 2 của MediaItem
    {
        // Gán các trường của IMedia theo yêu cầu đề bài
        _creator = "N/A";
        _releaseDate = DateTime.Now;
        _duration = 0;
    }

    // Constructor 2: (int code, string name, string creator, DateTime releaseDate, int duration)
    public Movie(int code, string name, string creator, DateTime releaseDate, int duration)
        : base(code, name) // Gọi constructor 2 của MediaItem
    {
        // Gán các trường của IMedia
        _creator = creator;
        _releaseDate = releaseDate;
        _duration = duration;
    }

    // Method DisplayInfo(): in thông tin ra dạng:
    // Code: 101 | Title: Inception | Director: Nolan | Release: 2010 | Length: 148 mins.
    public override void DisplayInfo()
    {
        Console.WriteLine($"Code: {Code} | Title: {Name} | Director: {Creator} | Release: {ReleaseDate.Year} | Length: {Duration} mins.");
    }
}

// ========== CÂU 3: Nhập liệu và Lưu trữ ==========

public static class MovieDB
{
    // Hàm static: Nhập từ bàn phím danh sách Movie (List<Movie>)
    public static void InputMovies(List<Movie> movieList)
    {
        Console.WriteLine("\n--- BẮT ĐẦU NHẬP DANH SÁCH MOVIE ---");
        int code = 0;

        while (true)
        {
            // Nhập code. Nếu nhập -1 thì dừng.
            Console.Write($"\nNhập Code cho Movie (hoặc -1 để kết thúc): ");
            if (!int.TryParse(Console.ReadLine(), out code) || code == -1)
            {
                if (code == -1) break;
                // Xử lý ngoại lệ khi nhập sai định dạng
                Console.WriteLine("LỖI: Định dạng Code không hợp lệ. Vui lòng nhập số nguyên.");
                continue;
            }

            // Kiểm tra code đã tồn tại chưa
            if (movieList.Any(m => m.Code == code))
            {
                Console.WriteLine($"LỖI: Code {code} đã tồn tại trong danh sách. Vui lòng nhập Code khác.");
                continue;
            }

            try
            {
                // Các bước nhập dữ liệu Movie còn lại
                Console.Write("Nhập Tên Movie (Title): ");
                string name = Console.ReadLine();

                Console.Write("Nhập Tên Đạo diễn (Creator): ");
                string creator = Console.ReadLine();

                // Nhập năm phát hành (đơn giản hóa cho việc nhập liệu)
                Console.Write("Nhập Năm phát hành (Ví dụ: 2010): ");
                if (!int.TryParse(Console.ReadLine(), out int year))
                {
                    Console.WriteLine("LỖI: Định dạng Năm không hợp lệ. Sử dụng năm hiện tại.");
                    year = DateTime.Now.Year;
                }
                DateTime releaseDate = new DateTime(year, 1, 1);

                Console.Write("Nhập Thời lượng (Duration) (phút): ");
                if (!int.TryParse(Console.ReadLine(), out int duration))
                {
                    Console.WriteLine("LỖI: Định dạng Thời lượng không hợp lệ. Sử dụng thời lượng 0.");
                    duration = 0;
                }

                // Tạo đối tượng Movie và thêm vào danh sách
                Movie movie = new Movie(code, name, creator, releaseDate, duration);
                movieList.Add(movie);
                Console.WriteLine($"-> Đã thêm Movie '{movie.Name}' (Code: {movie.Code}) vào danh sách.");
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ do Property Name ném ra (nếu tên < 3 ký tự)
                Console.WriteLine($"LỖI khi tạo Movie: {ex.Message}");
            }
        }
        Console.WriteLine("--- KẾT THÚC NHẬP DANH SÁCH MOVIE ---");
    }

    // Lưu dữ liệu vào CSDL MovieDB bằng Movie(Code, Name, Creator, ReleaseDate, Duration)
    // Giả lập lưu vào tệp (file) thay vì CSDL thực tế.
    public static void SaveMoviesToDatabase(List<Movie> movieList, string filePath = "MovieDB.txt")
    {
        try
        {
            // Lớp StreamWriter giúp ghi dữ liệu vào tệp.
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Ghi tiêu đề (header) của các cột
                writer.WriteLine("Code,Name,Creator,ReleaseDate,Duration");

                // Duyệt qua từng đối tượng Movie trong danh sách
                foreach (var movie in movieList)
                {
                    // Ghi thông tin của Movie dưới dạng chuỗi, ngăn cách bởi dấu phẩy (CSV-like format)
                    writer.WriteLine($"{movie.Code},{movie.Name},{movie.Creator},{movie.ReleaseDate.ToShortDateString()},{movie.Duration}");
                }
            }
            Console.WriteLine($"\nĐÃ LƯU: {movieList.Count} Movie vào 'CSDL' tại tệp '{filePath}' thành công.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"LỖI khi lưu dữ liệu vào tệp: {ex.Message}");
        }
    }
}

// Lớp chính để chạy chương trình
public class Program
{
    public static void Main(string[] args)
    {
        List<Movie> movieList = new List<Movie>();

        // 1. Nhập liệu (Câu 3)
        MovieDB.InputMovies(movieList);

        // 2. Hiển thị thông tin các Movie đã nhập (kiểm tra DisplayInfo)
        Console.WriteLine("\n--- DANH SÁCH MOVIE ĐÃ NHẬP ---");
        if (movieList.Count == 0)
        {
            Console.WriteLine("Không có Movie nào được nhập.");
        }
        else
        {
            foreach (var movie in movieList)
            {
                movie.DisplayInfo();
            }
        }

        // 3. Lưu trữ (Câu 3)
        MovieDB.SaveMoviesToDatabase(movieList);

        // Giữ cửa sổ console mở
        Console.WriteLine("\nNhấn phím bất kỳ để thoát...");
        Console.ReadKey();
    }
}