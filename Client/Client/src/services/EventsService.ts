import {AxiosResponse} from "axios";
import $api from "../http";
import {IEvent, IEventCreate} from "../models/Event";

export default  class EventsService{
    static fetchEvents(page:number):Promise<AxiosResponse>{
        return $api.get<AxiosResponse<IEvent>>(`/Events/events/${page}`);
    }
    static fetchEvent(id: number): Promise<AxiosResponse>{
        return $api.get<AxiosResponse<IEvent>>(`/Events/${id}`);
    }
    static createEvent(event: IEventCreate): Promise<AxiosResponse> {
        
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
    
    static getEvetnsByUserId(id:number):Promise<AxiosResponse>{
        return $api.get<AxiosResponse<IEvent[]>>(`/Events/user-events/${id}`);
    }
    
    static deleteEventParticipant(eventId:number, userId:number):Promise<AxiosResponse>{
        return $api.delete<AxiosResponse>(`/Participants/${eventId}/cancel/${userId}`);
    }
    
    static getParticipants(eventId:number):Promise<AxiosResponse>{
        return $api.get<AxiosResponse>(`/Participants/${eventId}/participants`);
    }
    
} 