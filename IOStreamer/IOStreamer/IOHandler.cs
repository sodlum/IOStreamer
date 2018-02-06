using System;
using System.Text;
using System.IO;

namespace IOStreamer
{
    /// <summary>
    /// Provides API to easily read/write file data
    /// </summary>
    public class IOHandler
    {
        private string _file;
        private FileStream _fs;

        public string FilePath { get; private set; }

        public IOHandler(string path, string file)
        {
            this.FilePath = path;
            this._file = file;
        }

        /// <summary>
        /// Create response object for IO operations
        /// </summary>
        /// <param name="success">Operation successful?</param>
        /// <param name="message">Operation details</param>
        /// <returns>Response object to be returned to API caller</returns>
        private IOHandlerResponse GetResponse(bool success, string message)
        {
            var response = new IOHandlerResponse
            {
                IsSuccess = success,
                Message = message
            };

            return response;
        }

        /// <summary>
        /// Opens file for reading/writing
        /// </summary>
        /// <returns>Response object with either success or exception message</returns>
        private IOHandlerResponse OpenStream()
        {
            try
            {
                this._fs = new FileStream(Path.Combine(this.FilePath, this._file), FileMode.OpenOrCreate, FileAccess.ReadWrite);
                return GetResponse(true, "Success");
            }
            catch (Exception ex)
            {
                return GetResponse(false, ex.Message);
            }
        }

        /// <summary>
        /// Closing stream and stops all read/write operations
        /// </summary>
        /// <returns>Response object with either success or exception message</returns>
        private IOHandlerResponse CloseStream()
        {
            try
            {
                this._fs.Dispose();
                this._fs = null;

                return GetResponse(true, "Success");
            }
            catch (Exception ex)
            {
                return GetResponse(false, ex.Message);
            }
        }

        /// <summary>
        /// Read data from currently open stream
        /// </summary>
        /// <returns>Response object with either success or exception message</returns>
        public IOHandlerResponse ReadStream()
        {
            IOHandlerResponse response;

            try
            {
                this.OpenStream();

                var length = (int)this._fs.Length;
                var buffer = new byte[length];
                var sum = 0;
                int count;

                while ((count = this._fs.Read(buffer, sum, length - sum)) > 0)
                {
                    sum += count;
                }

                var data = Encoding.ASCII.GetString(buffer);

                response = GetResponse(true, data);
            }
            catch (Exception ex)
            {
                response = GetResponse(false, ex.Message);
            }
            finally
            {
                this.CloseStream();
            }

            return response;
        }

        /// <summary>
        /// Write data to currently open stream
        /// </summary>
        /// <param name="data">Output contents</param>
        /// <returns>Response object with either success or exception message</returns>
        public IOHandlerResponse WriteStream(string data)
        {
            IOHandlerResponse response;

            try
            {
                this.OpenStream();

                byte[] buffer = Encoding.ASCII.GetBytes(data);

                this._fs.Write(buffer, 0, buffer.Length);

                response = GetResponse(true, "Success");
            }
            catch (Exception ex)
            {
                response = GetResponse(false, ex.Message);
            }
            finally
            {
                this.CloseStream();
            }

            return response;
        }
    }

    /// <summary>
    /// Response object to be returned to API callers
    /// </summary>
    public class IOHandlerResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
