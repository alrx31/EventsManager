export interface IEvent{
    id:number;
    
    name:string;
    description:string;
    date:Date;
    location:string;
    category:string;
    maxParticipants:number;
    imageSrc:string;
    
}

export interface IEventCreate {
    name: string;
    description: string;
    date: Date;
    location: string;
    category: string;
    maxParticipants: number;
    imageData: File;
}