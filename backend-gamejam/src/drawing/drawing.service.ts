import { Injectable } from "@nestjs/common";
import { InjectRepository } from "@nestjs/typeorm";
import { DrawingRepository } from "./drawing.repository";
import { Drawing } from "./drawing.entity";
import { DrawingDto } from "./drawing.dto";

@Injectable()
export class DrawingService {
    private readonly MAX_RANDOM_DRAWINGS = 100;

    constructor(
        @InjectRepository(DrawingRepository)
        private readonly drawingRepository: DrawingRepository
    ) {}

    

    async createDrawing(drawingDto: DrawingDto): Promise<Drawing> {
        return await this.drawingRepository.createDrawing(drawingDto);
    }

    async getDrawingById(id: number): Promise<Drawing | undefined> {
        return await this.drawingRepository.findDrawingById(id);
    }

    async getDrawingsByUserName(userName: string): Promise<Drawing[]> {
        return await this.drawingRepository.findByUserName(userName);
    }

    async updateDrawing(id: number, updateData: Partial<DrawingDto>): Promise<Drawing> {
        return await this.drawingRepository.updateDrawing(id, updateData);
    }

    async deleteDrawing(id: number): Promise<void> {
        await this.drawingRepository.deleteDrawing(id);
    }

    async getLast10Drawings(): Promise<Drawing[]> {
        return await this.drawingRepository.findLast10Drawings();
    }

    async getFirst10Drawings(): Promise<Drawing[]> {
        return await this.drawingRepository.findFirst10Drawings();
    }
    async getRandomDrawings(limit: number): Promise<Drawing[]> {
        const effectiveLimit = Math.min(limit, this.MAX_RANDOM_DRAWINGS);
        return await this.drawingRepository.findRandomDrawings(effectiveLimit);
    }
}