using API.DTO;
using API.Interfaces;
using API.Types;


namespace API.Utils
{
	public class ResponseBuilder<T> : IResponseBuilder<T>
	{
		private T data = default(T);
		private PaginationDTO pagination = null;
		private int statusCode = 200;
		private bool status = true;

		public IResponseBuilder<T> AddData(T data)
		{
			this.data = data;
			return this;
		}

		public IResponseBuilder<T> AddHttpStatus(int statusCode, bool status)
		{
			this.status = status;
			this.statusCode = statusCode;
			return this;
		}

		public IResponseBuilder<T> AddPagination(PaginationDTO pagination)
		{
			this.pagination = pagination;
			return this;
		}

		public Response<T> Build()
		{
			return new Response<T>
			{
				Data = this.data,
				pagination = this.pagination,
				status = this.statusCode,
				success = this.status
			};
		}


	}
}