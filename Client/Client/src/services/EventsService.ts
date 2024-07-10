import {AxiosResponse} from "axios";
import $api from "../http";
import {IEvent, IEventCreate} from "../models/Event";

export default  class EventsService{
    static async fetchEvents(page:number,pageSize:number):Promise<AxiosResponse>{
        return $api.get<AxiosResponse<IEvent[]>>(`/Events/page=${page}&pageSize=${pageSize}`);
    }
    static async fetchEvent(id: number): Promise<AxiosResponse>{
        return $api.get<AxiosResponse<IEvent>>(`/Events/${id}`);
    }
    static async createEvent(event: IEventCreate): Promise<AxiosResponse> {
        
        const formData = new FormData();
        formData.append("Name", event.name);
        formData.append("Description", event.description);
        formData.append("Date", event.date.toISOString());
        formData.append("Location", event.location);
        formData.append("Category", event.category);
        formData.append("MaxParticipants", event.maxParticipants.toString());
        formData.append("ImageData", event.imageData);
        
        console.log(formData)
        return $api.post<AxiosResponse>('/Events/create-event', formData);
    }
    
    static async getEvetnsByUserId(id:number):Promise<AxiosResponse>{
        return $api.get<AxiosResponse<IEvent[]>>(`/Events/user-events/${id}`);
    }
    
    static async deleteEventParticipant(eventId:number, userId:number):Promise<AxiosResponse>{
        return $api.delete<AxiosResponse>(`/Participants/${eventId}/cancel/${userId}`);
    }
    
    static async getParticipants(eventId:number):Promise<AxiosResponse>{
        return $api.get<AxiosResponse>(`/Participants/${eventId}/participants`);
    }
    static async searchEvents(
        NameS:string,
        DateS:Date,
        pageNum:number,
        perPage:number
    ):Promise<AxiosResponse> {
        let dataS = {
            "date":DateS,
            "name":NameS
        }
        return $api.post<AxiosResponse>(`Events/search&page=${pageNum}&pageSize=${perPage}`,dataS);
    }
    
    static async getCountEvents():Promise<AxiosResponse>{
        return $api.get<AxiosResponse>(`/Events/count`);
    }
    static async getCountEventsSearch(NameS:string, DateS:Date):Promise<AxiosResponse>{
        let dataS = {
            "date":DateS,
            "name":NameS
        }
        return $api.post<AxiosResponse>(`Events/search/count`,dataS);
    }
    
    static async filterEvents(location:string,category:string,page:number,pageSize:number):Promise<AxiosResponse>{
        let data = {
            'location':location,
            'category':category
        }
        return $api.post<AxiosResponse>(`/events/filter&page=${page}&pageSize=${pageSize}`,data);
    }
    
    static async getFilterEventsCount(location:string,category:string):Promise<AxiosResponse>{
        let data = {
            'location':location,
            'category':category
        }
        return $api.post<AxiosResponse<number>>(`events/filter/count`, data);
    }
    
    static async deleteEvent(id:number):Promise<AxiosResponse>{
        return $api.delete<AxiosResponse>(`/Events/${id}`);
    }
    
    static async updateEvent(event:IEventCreate,eventId:number):Promise<AxiosResponse>{
        const formData = new FormData();
        formData.append("Name", event.name);
        formData.append("Description", event.description);
        formData.append("Date", event.date.toISOString());
        formData.append("Location", event.location);
        formData.append("Category", event.category);
        formData.append("MaxParticipants", event.maxParticipants.toString());
        formData.append("ImageData", event.imageData);
        console.log(formData)
        return $api.put<AxiosResponse>(`/Events/${eventId}`,formData);
    }
    
} 