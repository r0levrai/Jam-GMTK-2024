import { BadRequestException, Injectable, NotFoundException } from "@nestjs/common";
import { InjectRepository } from "@nestjs/typeorm";
import { DrawingRepository } from "./drawing.repository";
import { convertDrawingToDto, Drawing, ReactionType } from "./drawing.entity";
import { DrawingDto } from "./drawing.dto";
import { DrawingResponseDto } from "./drawing.response.dto";
import { PageOptionsDto } from "src/config/page/page-option.dto";
import { PageMetaDto } from "src/config/page/page-meta.dto";
import { PageDto } from "src/config/page/page.dto";

@Injectable()
export class DrawingService {
    private readonly MAX_RANDOM_DRAWINGS = 100;

    constructor(
        @InjectRepository(DrawingRepository)
        private readonly drawingRepository: DrawingRepository
    ) {}

    

    async createDrawing(drawingDto: DrawingDto): Promise<DrawingResponseDto> {
        return convertDrawingToDto(await this.drawingRepository.createDrawing(drawingDto));
    }

    async getDrawingById(id: number): Promise<DrawingResponseDto | undefined> {
        return convertDrawingToDto(await this.drawingRepository.findDrawingById(id));
    }

    async getDrawingsByUserName(userName: string): Promise<DrawingResponseDto[]> {
        return (await this.drawingRepository.findByUserName(userName)).map(convertDrawingToDto);
    }

    async updateDrawing(id: number, updateData: Partial<DrawingDto>): Promise<DrawingResponseDto> {
        return convertDrawingToDto(await this.drawingRepository.updateDrawing(id, updateData));
    }

    async deleteDrawing(id: number): Promise<void> {
        await this.drawingRepository.deleteDrawing(id);
    }


    
    async getRandomDrawings(pageOptionDto: PageOptionsDto): Promise<PageDto<DrawingResponseDto>> {
        const [items, totalItems] = (await this.drawingRepository.findRandomDrawings(pageOptionDto));
        const pageMetaDto = new PageMetaDto({ pageOptionsDto: pageOptionDto, itemCount: totalItems });
        try {
            return new PageDto(items.map(convertDrawingToDto), pageMetaDto);
        }
        catch (error) {
            console.error("Error fetching random drawings:", error);
            throw new Error("Could not fetch random drawings");
        }
    }
    
    async getAllDrawings(pageOptionsDto: PageOptionsDto): Promise<PageDto<DrawingResponseDto>> {
        const [items, totalItems] = await this.drawingRepository.findAllDrawings(pageOptionsDto);
        const pageMetaDto = new PageMetaDto({ pageOptionsDto, itemCount: totalItems });
        try {
            return new PageDto(items.map(convertDrawingToDto), pageMetaDto);
        } catch (error) {
            console.error("Error fetching all drawings:", error);
            throw new Error("Could not fetch all drawings");
        }
    }

    async getDrawingsOrderedByLikes(pageOptionsDto: PageOptionsDto): Promise<PageDto<DrawingResponseDto>> {
        const [items, totalItems] = await this.drawingRepository.findDrawingsOrderedByLikes(pageOptionsDto);
        const pageMetaDto = new PageMetaDto({ pageOptionsDto, itemCount: totalItems });
        try {
            return new PageDto(items.map(convertDrawingToDto), pageMetaDto);
        } catch (error) {
            console.error("Error fetching drawings ordered by likes:", error);
            throw new Error("Could not fetch drawings ordered by likes");
        }
    }

    async getLastDrawings(limit: number): Promise<DrawingResponseDto[]> {
        return (await this.drawingRepository.findLastDrawings(limit)).map(convertDrawingToDto);
    }

    async getFirstDrawings(limit: number): Promise<DrawingResponseDto[]> {
        return (await this.drawingRepository.findFirstDrawings(limit)).map(convertDrawingToDto);
    }


    
  

    async addReaction(drawingId: number, reaction: string, ipAddress: string): Promise<DrawingResponseDto> {
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
        return convertDrawingToDto(await this.drawingRepository.save(drawing));
    }
}