import { BadRequestException, Injectable, NotFoundException } from "@nestjs/common";
import { InjectRepository } from "@nestjs/typeorm";
import { DrawingRepository } from "./drawing.repository";
import { Drawing, ReactionType } from "./drawing.entity";
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

    async getLastDrawings(limit: number): Promise<Drawing[]> {
        return await this.drawingRepository.findLastDrawings(limit);
    }

    async getFirstDrawings(limit: number): Promise<Drawing[]> {
        return await this.drawingRepository.findFirstDrawings(limit);
    }
    async getRandomDrawings(limit: number): Promise<Drawing[]> {
        const effectiveLimit = Math.min(limit, this.MAX_RANDOM_DRAWINGS);
        return await this.drawingRepository.findRandomDrawings(effectiveLimit);
    }
  

    async addReaction(drawingId: number, reaction: string, ipAddress: string): Promise<Drawing> {
        const drawing = await this.drawingRepository.findDrawingById(drawingId)
        if (!drawing) {
            throw new NotFoundException("Not found");
        }

        if (reaction === ReactionType.LIKE) {
            if (drawing.like.includes(ipAddress))
                throw new BadRequestException("already like")
            drawing.like.push(ipAddress)
        }
        if (reaction === ReactionType.FUNNY) {
            if (drawing.funny.includes(ipAddress))
                throw new BadRequestException("already funny")
            drawing.funny.push(ipAddress)
        }
        if (reaction === ReactionType.BAD) {
            if (drawing.bad.includes(ipAddress))
                throw new BadRequestException("already bad")
            drawing.bad.push(ipAddress)
        }
        return await this.drawingRepository.save(drawing);
    }
}