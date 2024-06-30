import {IEvent} from "../models/Event";
import {AxiosResponse} from "axios";
import $api from "../http";

export default class ParticipantService {
    static async CreteParticipant(eventId:number, userId:number):Promise<AxiosResponse>{
        return $api.put(`/Participants/${eventId}/register/${userId}`);
    }
    
    static async CanselParticipant(eventId:number,userId:number):Promise<AxiosResponse>{
        return $api.delete(`/Participants/${eventId}/register/${userId}`);
    }
    
    static async fetchParticipants(eventId:number):Promise<AxiosResponse>{
        return $api.get(`/Participants/${eventId}/participants`);
    }
}