import {AxiosResponse} from "axios";
import $api from "../http";
export default class UserService {
    static async fetchUserById(id:number):Promise<AxiosResponse> {
        return $api.get(`/Participants/${id}`);
    }
}