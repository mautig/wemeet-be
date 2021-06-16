using API.Entities;
using API.Types;
using API.DTO;
using API.Models;
using System.Threading.Tasks;
using System;

namespace API.Interfaces
{
	public interface IRoomRepository
	{
		void AddOne(RoomDTO info);
		Task<Pagination<RoomDTO>> GetAllByPaginationAsync(PaginationParams paginationQuery);
		Task<RoomDTO> GetOneAsync(string Id);
		int GetSizeOfEntity(Func<Room, bool> query);
		void DeletingOne(string Id);
		void UpdatingOne(RoomDTO room);
	}
}